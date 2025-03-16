using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Build.Utilities;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.Git;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Git;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Tools.ReportGenerator;
using Serilog;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using Nuke.Common.Tools.MinVer;

[AzurePipelines(
    AzurePipelinesImage.UbuntuLatest,
    // AzurePipelinesImage.WindowsLatest,
    AutoGenerate = true,
    InvokedTargets = [nameof(PublishToNuGet)],
    NonEntryTargets = [nameof(Restore)])]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.PublishToNuGet);

    #region Fields

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion(Framework = "net8.0")] readonly GitVersion GitVersion;

    AbsolutePath BinDirectory => RootDirectory / "bin" / Configuration;
    AbsolutePath PackagesDirectory => RootDirectory / "packages";
    AbsolutePath PackagesPropsPath => RootDirectory / "Directory.Packages.props";
    AbsolutePath TestResultsDirectory => RootDirectory / "test-results" / Configuration;
    AbsolutePath CoverageDirectory => TestResultsDirectory / "coverage-results";
    AbsolutePath ReportDirectory => TestResultsDirectory / "summery-report";

    bool IsDeveloperMode => Configuration == Configuration.Debug;

    #endregion

    #region Parameters

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Nuget api key")] [Secret] readonly string NuGetApiKey;
    [Parameter("Github secure token")] [Secret] readonly string GitHubToken;
    
    [Parameter("The git command")] readonly string GitCommand = "add .";
    [Parameter] readonly string CommitMessage;
    [Parameter] readonly string GitHubBranch;
    [Parameter] readonly string GitHubUsername;


    [Parameter("New version for packages")] readonly string NewVersion = "3.0.1";

    [Parameter("Prefix of packages to update")] readonly string PackagePrefix = "Kharazmi.";

    [Parameter("Packages to update. Example: 'Kharazmi.AspNetCore: 3.0.0, Kharazmi.EfCore: 3.0.1' ")]
    readonly string PackagesToUpdate = "";

    [Parameter("Major version")] readonly int MajorVersion;

    [Parameter("Increment major version")] readonly bool IncrementMajorVersion;

    [Parameter("Minor version")] readonly int MinorVersion;

    [Parameter("Increment minor version")] readonly bool IncrementMinorVersion;

    [Parameter("Patch version")] readonly int PatchVersion;

    [Parameter("Increment Patch Version")] readonly bool IncrementPatchVersion;

    [Parameter("Version prefix (e.g. alpha, beta)")] readonly string VersionPrefix = "";

    [Parameter("Version suffix (e.g. alpha.1, beta.2)")] readonly string VersionSuffix = "";

    [Parameter("Version maintenance file")] readonly string VersionFile = "version.json";

    #endregion

    Target Clean => _ => _
        .Description("Clear previous projects and outputs")
        .Before(Restore)
        .Executes(() =>
        {
            DotNetClean(_ => _
                .SetProject(Solution)
                .SetConfiguration(Configuration));

            if (!IsDeveloperMode) return;
            TestResultsDirectory.DeleteDirectory();
            CoverageDirectory.DeleteDirectory();
            ReportDirectory.DeleteDirectory();
        });


    Target Restore => _ => _
        .Description("Restore NuGet packages")
        .OnlyWhenDynamic(() => IsDeveloperMode)
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .Description("Compile projects")
        .OnlyWhenDynamic(() => IsDeveloperMode)
        .DependsOn(Clean)
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .Description("Running tests")
        .DependsOn(Compile)
        .Executes(() =>
        {
            CoverageDirectory.CreateOrCleanDirectory();

            var testProjects = Solution.GetAllProjects("*.XUnitTests");
            foreach (var project in testProjects)
            {
                var targetFramework = project.GetTargetFrameworks()?.First();
                var projectName = project.Name;

                DotNetTest(_ => _
                    .SetProjectFile(project)
                    .SetConfiguration(Configuration)
                    .EnableNoRestore()
                    .EnableNoBuild()
                    .SetResultsDirectory(TestResultsDirectory)
                    .SetLoggers($"trx;LogFileName={projectName}.trx")
                    .SetVerbosity(DotNetVerbosity.normal));

                CoverletTasks.Coverlet(s => s
                    .SetTarget("dotnet")
                    .SetTargetArgs("test", Solution.Path, "--no-build", "--no-restore")
                    .SetAssembly(project.Directory / "bin" / Configuration / targetFramework / projectName + ".dll")
                    // .SetThreshold(75)
                    .SetOutput(CoverageDirectory / "opencover.xml")
                    .SetFormat(CoverletOutputFormat.opencover)
                    .SetExclude("*Tests"));
            }
        });

    Target Report => _ => _
        .Description("Report Testing")
        .DependsOn(Test)
        .AssuredAfterFailure()
        .Executes(() =>
        {
            ReportDirectory.CreateOrCleanDirectory();

            ReportGeneratorTasks.ReportGenerator(s => s
                .SetTargetDirectory(ReportDirectory)
                .SetFramework("net8.0")
                .SetReportTypes(ReportTypes.Html)
                .SetReports(CoverageDirectory / "opencover.xml"));
        });

    Target SetupLocalFeed => _ => _
        .Description("Add `LocalPackages` as new nuget source")
        .Before(Pack)
        .OnlyWhenDynamic(() => Configuration == Configuration.Release)
        .Executes(() =>
        {
            try
            {
                NuGetTasks.NuGetSourcesAdd(x => x
                    .SetName("LocalPackages")
                    .SetConfigFile(RootDirectory / "nuget.config")
                    .SetSource(PackagesDirectory));
            }
            catch (Exception)
            {
                Log.Warning("""

                            ╬═════════
                            ║ The LocalPackages as nuget source is already exist.
                            ╬══════════════════════════════════════

                            """);
            }
        });

    Target Pack => _ => _
        .Description("Library Packaging")
        .OnlyWhenDynamic(() => Configuration == Configuration.Release)
        .DependsOn(SetupLocalFeed)
        .Executes(() =>
        {
            PackagesDirectory.CreateDirectory();

            var packableProjects = Solution
                .GetAllProjects("*")
                .Where(p => !p.Name.EndsWith("Tests") && !p.Name.EndsWith("Console") && !p.Name.EndsWith("_build"));

            var sortedProjects = SortProjectsByDependencies(packableProjects);
            var versions = Versions.LoadProjectVersions(RootDirectory, VersionFile);
            var versionOption = CreateVersionOptions();

            XDocument? xDoc = null;
            XElement[]? packageElements = null;
            if (File.Exists(PackagesPropsPath))
            {
                xDoc = XDocument.Load(PackagesPropsPath);
                packageElements = xDoc.Root?.Elements("ItemGroup")
                    .Where(ig => ig.Elements("PackageVersion").Any())
                    .SelectMany(c => c.Elements())
                    .ToArray();
            }


            foreach (var project in sortedProjects)
            {
                Log.Information("""

                                ╬═════════
                                ║ Rebuild {Project}
                                ╬══════════════════════════════════════

                                """, project);

                var version = versions.GetOrDefaultVersion(project.Name);
                version.ValidateVersion(versionOption);

                var fullVersion = version.FullVersion;

                DotNetMSBuild(x => x
                    .SetTargetPath(project)
                    .SetTargets("rebuild")
                    .SetConfiguration(Configuration)
                    .SetAssemblyVersion(fullVersion)
                    .SetFileVersion(fullVersion)
                    .SetInformationalVersion(fullVersion)
                );

                Log.Information("""

                                ╬═════════
                                ║ Pack {Project}
                                ╬══════════════════════════════════════

                                """, project);

                DotNetPack(_ => _
                    .SetProject(project)
                    .SetConfiguration(Configuration)
                    .EnableNoBuild()
                    .EnableNoRestore()
                    .EnableNoCache()
                    .SetSources(RootDirectory / "nuget.config")
                    .SetVersion(fullVersion)
                    .SetInformationalVersion(CommitMessage)
                    .SetOutputDirectory(PackagesDirectory));

                versions.AddOrUpdateVersion(version, project.Name);

                var packageId = project.Name;

                if (!UpdatePackageVersion(packageElements, packageId, fullVersion, out var packageInfo))
                {
                    Log.Error("Package {PackageId} not found in {PackagesPropsPath}", packageId, PackagesPropsPath);
                }
                else
                {
                    Log.Information("Updated {PackageId}: {CurrentVersion} -> {NewVersion}",
                        packageId, packageInfo.Value.CurrentVersion, fullVersion);
                    xDoc?.Save(PackagesPropsPath);
                }
            }

            versions.UpdateGlobalVersion(versionOption);
            versions.SaveVersions(RootDirectory, VersionFile);

            DotNetClean(_ => _
                .SetProject(Solution)
                .SetConfiguration(Configuration));

            DotNetRestore(_ => _
                .SetProjectFile(Solution));
        });

    Action<VersionOption> CreateVersionOptions() => op =>
    {
        op.Major = MajorVersion;
        op.Minor = MinorVersion;
        op.Patch = PatchVersion;
        op.IncrementMajor = IncrementMajorVersion;
        op.IncrementMinor = IncrementMinorVersion;
        op.IncrementPatch = IncrementPatchVersion;
        op.VersionPrefix = VersionPrefix;
        op.VersionSuffix = VersionSuffix;
        op.VersionFile = VersionFile;
    };

    Target PublishToNuGet => _ => _
        .Description("Publish packages to NuGet")
        .OnlyWhenDynamic(() => Configuration == Configuration.Release)
        .DependsOn(Pack)
        .Requires(() => !string.IsNullOrWhiteSpace(NuGetApiKey))
        .Executes(() =>
        {
            var packages = PackagesDirectory.GlobFiles("*.nupkg");

            foreach (var package in packages)
            {
                DotNetNuGetPush(_ => _
                    .SetTargetPath(package)
                    .SetApiKey(NuGetApiKey)
                    .SetSource("https://api.nuget.org/v3/index.json")
                    .SetSkipDuplicate(true));
            }

            Log.Information($"{packages.Count} packages successfully published to NuGet.");
        });

    Target UpdatePackages => _ => _
        .Description("Update packages with specific package prefix")
        .OnlyWhenDynamic(() =>
            !string.IsNullOrEmpty(PackagePrefix) && !string.IsNullOrEmpty(NewVersion) && !IsDeveloperMode)
        .Executes(() =>
        {
            if (!File.Exists(PackagesPropsPath))
            {
                Log.Error($"File not found: {PackagesPropsPath}");
                return;
            }

            var xDoc = XDocument.Load(PackagesPropsPath);
            var packageElements = xDoc.Root?.Elements("ItemGroup")
                .Where(ig => ig.Elements("PackageVersion").Any())
                .SelectMany(c => c.Elements())
                .ToArray();

            if (packageElements == null)
            {
                Log.Error("ItemGroup with PackageVersion elements not found");
                return;
            }

            foreach (var packageElement in packageElements)
            {
                var packageId = packageElement.Attribute("Include")?.Value;
                if (string.IsNullOrEmpty(packageId)) continue;

                if (!packageId.StartsWith(PackagePrefix, StringComparison.InvariantCultureIgnoreCase)) continue;

                var currentVersion = packageElement.Attribute("Version")?.Value;
                if (currentVersion is null ||
                    currentVersion.Equals(NewVersion, StringComparison.InvariantCultureIgnoreCase))
                {
                    Log.Warning("No need to be Updated {PackageId}: {CurrentVersion} -> {NewVersion}",
                        packageId, currentVersion, NewVersion);

                    continue;
                }

                packageElement.SetAttributeValue("Version", NewVersion);
                Log.Information("Updated {PackageId}: {CurrentVersion} -> {NewVersion}",
                    packageId, currentVersion, NewVersion);
            }

            xDoc.Save(PackagesPropsPath);
        });

    Target UpdateSpecificPackage => _ => _
        .Description("Update the version of a specific package")
        .Executes(() =>
        {
            if (!File.Exists(PackagesPropsPath))
            {
                Log.Error($"File not found: {PackagesPropsPath}");
                return;
            }

            var xDoc = XDocument.Load(PackagesPropsPath);
            var packageElements = xDoc.Root?.Elements("ItemGroup")
                .Where(ig => ig.Elements("PackageVersion").Any())
                .SelectMany(c => c.Elements())
                .ToArray();

            var packages = PackagesToUpdate.Split(',').AsSpan();
            foreach (var package in packages)
            {
                var packageAndVersion = package.Split(':');
                var packageId = packageAndVersion[0];
                var newVersion = packageAndVersion[1];
                if (!UpdatePackageVersion(packageElements, packageId, newVersion, out var packageInfo))
                {
                    Log.Error("Package {PackageId} not found in {PackagesPropsPath}", packageId, PackagesPropsPath);
                }
                else
                {
                    Log.Information("Updated {PackageId}: {CurrentVersion} -> {NewVersion}",
                        packageId, packageInfo.Value.CurrentVersion, newVersion);
                }
            }

            xDoc.Save(PackagesPropsPath);
        });

    public readonly record struct UpdatePackageInfo(string PackageId, string? CurrentVersion);


    Target CommitChanges => _ => _
        .Description("Check changes in Git")
        .OnlyWhenDynamic(() => IsDeveloperMode)
        .Requires(() => !string.IsNullOrWhiteSpace(CommitMessage))
        .Executes(() =>
        {
            Log.Information(GitTasks.GitPath);

            var statusResult = GitTasks.Git("status --porcelain").FirstOrDefault();
            if (string.IsNullOrEmpty(statusResult.Text))
            {
                Log.Information("There are no changes to commit.");
                return;
            }

            GitTasks.Git(GitCommand);

            Log.Information($"Commit Message: \"{CommitMessage}\"");

            GitTasks.Git($"commit -m '{CommitMessage}'");
            GitTasks.Git("log --oneline");

            Log.Information($"Changes committed with message \"{CommitMessage}\".");
        });

    Target PushToGitHub => _ => _
        .Description("Submit changes to GitHub")
        .OnlyWhenDynamic(() => IsDeveloperMode)
        .DependsOn(CommitChanges)
        .Requires(() => !string.IsNullOrWhiteSpace(GitHubBranch))
        .Requires(() => !string.IsNullOrWhiteSpace(GitHubUsername))
        .Requires(() => !string.IsNullOrWhiteSpace(GitHubToken))
        .Executes(() =>
        {
            var gitHubUrl =
                $"https://{GitHubUsername}:{GitHubToken}@github.com/{GitHubUsername}/{GitRepository.GetGitHubName()}.git";

            try
            {
                GitTasks.Git($"remote add github {gitHubUrl}");
            }
            catch
            {
                GitTasks.Git($"remote set-url github {gitHubUrl}");
            }

            GitTasks.Git($"push github HEAD:{GitHubBranch}");

            Log.Information($"Changes successfully pushed to branch {GitHubBranch} on GitHub.");
        });

    Target BuildDebug => _ => _
        .Description("Full workflow execution: Clean, Restore, Compile, Test, Report")
        .OnlyWhenDynamic(() => IsDeveloperMode)
        .DependsOn(Report)
        .Executes(() =>
        {
            Log.Information("جریان کاری کامل با موفقیت انجام شد!");
        });

    Target BuildRelease => _ => _
        .Description("اجرای کامل جریان کاری: Pack, PublishToNuGet")
        .OnlyWhenDynamic(() => !IsDeveloperMode)
        .DependsOn(PublishToNuGet)
        .Executes(() =>
        {
            Log.Information("The entire workflow was completed successfully!");
        });


    private static List<Project> SortProjectsByDependencies(IEnumerable<Project> projects)
    {
        var result = new List<Project>();
        var projectsList = projects.ToList();
        var visited = new HashSet<string>();
        var temp = new HashSet<string>();

        foreach (var project in projectsList)
        {
            if (!visited.Contains(project.Name))
            {
                Visit(project, projectsList, visited, temp, result);
            }
        }

        // result.Reverse();
        return result;
    }

    private static void Visit(Project project, List<Project> allProjects,
        HashSet<string> visited, HashSet<string> temp, List<Project> result)
    {
        if (temp.Contains(project.Name))
        {
            throw new Exception($"Circular dependency detected: {project.Name}");
        }

        if (visited.Contains(project.Name))
        {
            return;
        }

        temp.Add(project.Name);

        var dependencies = GetProjectReferences(project)
            .Select(reference => allProjects.FirstOrDefault(p => p.Path == reference))
            .Where(p => p != null);

        foreach (var dependency in dependencies)
        {
            if (!visited.Contains(dependency.Name))
            {
                Visit(dependency, allProjects, visited, temp, result);
            }
        }

        temp.Remove(project.Name);
        visited.Add(project.Name);
        result.Add(project);
    }

    private static IEnumerable<string> GetProjectReferences([JetBrains.Annotations.NotNull] Project project)
    {
        ArgumentNullException.ThrowIfNull(project);

        var projectInstance = new Microsoft.Build.Evaluation.Project(project.Path);
        return projectInstance.GetItems("ProjectReference")
            .Select(item => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(project.Path), item.EvaluatedInclude)));
    }


    bool UpdatePackageVersion(XElement[]? packageElements, string packageName,
        string newVersion, [NotNullWhen(true)] out UpdatePackageInfo? updatePackageInfo)
    {
        if (packageElements == null)
        {
            updatePackageInfo = null;
            return false;
        }

        foreach (var packageElement in packageElements)
        {
            var pkgId = packageElement.Attribute("Include")?.Value;
            if (string.IsNullOrEmpty(pkgId)) continue;

            if (!pkgId.Equals(packageName, StringComparison.InvariantCultureIgnoreCase)) continue;

            var currentVersion = packageElement.Attribute("Version")?.Value;
            if (currentVersion is null ||
                currentVersion.Equals(NewVersion, StringComparison.InvariantCultureIgnoreCase))
            {
                updatePackageInfo = new UpdatePackageInfo(pkgId, currentVersion);
                return false;
            }

            packageElement.SetAttributeValue("Version", newVersion);
            updatePackageInfo = new UpdatePackageInfo(pkgId, currentVersion);
            return true;
        }

        updatePackageInfo = null;
        return false;
    }
}

public sealed record VersionOption
{
    public string VersionFile { get; set; } = "version.json";
    public bool IncrementMajor { get; set; }
    public bool IncrementMinor { get; set; }
    public bool IncrementPatch { get; set; }
    public int Major { get; set; } = 1;
    public int Minor { get; set; }
    public int Patch { get; set; }
    public string VersionPrefix { get; set; } = "";
    public string VersionSuffix { get; set; } = "";

    public VersionOption IncrementMajorVersion()
    {
        IncrementMajor = true;
        return this;
    }

    public VersionOption IncrementMinorVersion()
    {
        IncrementMinor = true;
        return this;
    }

    public VersionOption IncrementPatchVersion()
    {
        IncrementPatch = true;
        return this;
    }
}

public sealed class Versions : Dictionary<string, Version>
{
    public static Versions LoadProjectVersions(AbsolutePath rootDirectory, string versionFile)
    {
        var versionFilePath = rootDirectory / versionFile;

        if (!versionFilePath.FileExists())
        {
            var versions = new Versions { { "Global", new Version() } };
            versions.SaveVersions(rootDirectory, versionFile);
            return versions;
        }

        try
        {
            var json = File.ReadAllText(versionFilePath);

            if (string.IsNullOrEmpty(json) || string.IsNullOrWhiteSpace(json))
            {
                var globalVersions = new Versions { { "Global", new Version() } };
                globalVersions.SaveVersions(rootDirectory, versionFile);
                return globalVersions;
            }

            var versionJson = System.Text.Json.JsonDocument.Parse(json);
            var versions = System.Text.Json.JsonSerializer.Deserialize<Versions>(versionJson);

            if (versions is not null) return versions;
            Log.Warning("Can't load the project versions. Invalid Json File: {VersionFile}", versionFilePath);
            return new Versions { { "Global", new Version() } };
        }
        catch (Exception ex)
        {
            Log.Warning("Can't get the project version. Invalid Version: {Message}", ex.Message);

            return new Versions { { "Global", new Version() } };
        }
    }

    public Version GetOrDefaultVersion(string? projectName = null)
    {
        projectName ??= "Global";

        if (TryGetValue(projectName, out var version))
        {
            return version;
        }

        return TryGetValue("Global", out var globalVersion) ? globalVersion : new Version();
    }

    public Versions AddOrUpdateVersion(Version version, string projectName)
    {
        if (TryGetValue(projectName, out _))
        {
            this[projectName] = version;
            return this;
        }

        TryAdd(projectName, version);
        return this;
    }

    public Versions UpdateGlobalVersion(Action<VersionOption>? options)
    {
        if (!TryGetValue("Global", out var version)) return this;
        version.ValidateVersion(options);
        this["Global"] = version;

        return this;
    }

    public void SaveVersions(AbsolutePath rootDirectory, string versionFile)
    {
        var versionFilePath = rootDirectory / versionFile;
        var versionJson = System.Text.Json.JsonSerializer.Serialize(this);

        File.WriteAllText(versionFilePath, versionJson);
        Log.Information("Save ths versions is successful");
    }
}

public record struct Version
{
    public Version() : this(1, 0, 0, "", "")
    {
    }

    [System.Text.Json.Serialization.JsonConstructor]
    public Version(int major, int minor, int patch, string prefix, string suffix)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
        Prefix = prefix;
        Suffix = suffix;
    }

    public string FullVersion
    {
        get
        {
            var version = $"{Major}.{Minor}.{Patch}";

            if (!string.IsNullOrEmpty(Prefix))
            {
                version = $"{Prefix}-{version}";
            }

            if (!string.IsNullOrEmpty(Suffix))
            {
                version = $"{version}-{Suffix}";
            }

            return version;
        }
    }

    [System.Text.Json.Serialization.JsonInclude] public int Major { get; set; }
    [System.Text.Json.Serialization.JsonInclude] public int Minor { get; set; }
    [System.Text.Json.Serialization.JsonInclude] public int Patch { get; set; }
    [System.Text.Json.Serialization.JsonInclude] public string Prefix { get; set; }
    [System.Text.Json.Serialization.JsonInclude] public string Suffix { get; set; }

    public Version ValidateVersion(Action<VersionOption>? options)
    {
        VersionOption option = new();
        options?.Invoke(option);

        if (option.Major > 1 && option.Major != Major)
            Major = option.Major;

        if (option.Minor > 1 && option.Minor != Minor)
            Minor = option.Minor;

        if (option.Patch > 1 && option.Patch != Patch)
            Patch = option.Patch;

        if (!string.IsNullOrEmpty(option.VersionPrefix) && option.VersionPrefix != Prefix)
            Prefix = option.VersionPrefix;

        if (!string.IsNullOrEmpty(option.VersionSuffix) && option.VersionSuffix != Suffix)
            Suffix = option.VersionSuffix;

        if (option.IncrementMajor)
        {
            IncrementMajorVersion();
        }

        if (option.IncrementMinor)
        {
            IncrementMinorVersion();
        }

        if (option.IncrementPatch)
        {
            IncrementPatchVersion();
        }

        return this;
    }

    public Version IncrementMajorVersion()
    {
        Major += 1;
        return this;
    }

    public Version IncrementMinorVersion()
    {
        Minor += 1;
        return this;
    }

    public Version IncrementPatchVersion()
    {
        Patch += 1;
        return this;
    }


    public static implicit operator (int Major, int Minor, int Patch, string Prefix, string Suffix)(
        Version value)
    {
        return (value.Major, value.Minor, value.Patch, value.Prefix, value.Suffix);
    }

    public static implicit operator Version(
        (int Major, int Minor, int Patch, string Prefix, string Suffix) value)
    {
        return new Version(value.Major, value.Minor, value.Patch, value.Prefix, value.Suffix);
    }

    public void Deconstruct(out int major, out int minor, out int patch, out string prefix, out string suffix)
    {
        major = Major;
        minor = Minor;
        patch = Patch;
        prefix = Prefix;
        suffix = Suffix;
    }
}