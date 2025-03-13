using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.Git;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Git;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Tools.GitVersion;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[AzurePipelines(
    AzurePipelinesImage.UbuntuLatest,
    // AzurePipelinesImage.WindowsLatest,
    AutoGenerate = true,
    InvokedTargets = new[] { nameof(PublishToNuGet) },
    NonEntryTargets = new[] { nameof(Restore) })]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        : https://nuke.build/resharper
    ///   - JetBrains Rider            : https://nuke.build/rider
    ///   - Microsoft VisualStudio     : https://nuke.build/visualstudio
    ///   - Microsoft VSCode           : https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.PublishToNuGet);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Nuget api key")] [Secret] readonly string NuGetApiKey;
    [Parameter("Github secure token")] [Secret] readonly string GitHubToken;
    [Parameter] readonly string CommitMessage;
    [Parameter] readonly string GitHubBranch;
    [Parameter] readonly string GitHubUsername;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion(Framework = "net8.0")] readonly GitVersion GitVersion;

    AbsolutePath PackagesDirectory => RootDirectory / "packages";
    AbsolutePath TestResultsDirectory => RootDirectory / "test-results";

    Target Clean => _ => _
        .Description("پاک کردن پروژه‌ها و خروجی‌های قبلی")
        .Before(Restore)
        .Executes(() =>
        {
            // پاک کردن همه پروژه‌ها در Solution
            DotNetClean(_ => _
                .SetProject(Solution)
                .SetConfiguration(Configuration));

            // پاک کردن پوشه پکیج‌ها
            PackagesDirectory.DeleteDirectory();
            TestResultsDirectory.DeleteDirectory();
        });

    Target Restore => _ => _
        .Description("بازیابی پکیج‌های NuGet")
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .Description("کامپایل پروژه‌ها")
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
        .Description("اجرای آزمون‌ها")
        .DependsOn(Compile)
        .Executes(() =>
        {
            // پیدا کردن تمام پروژه‌های تست
            var testProjects = Solution.GetAllProjects("*.Tests");

            // اجرای تست‌ها برای هر پروژه تست
            foreach (var project in testProjects)
            {
                DotNetTest(_ => _
                    .SetProjectFile(project)
                    .SetConfiguration(Configuration)
                    .EnableNoRestore()
                    .EnableNoBuild()
                    .SetResultsDirectory(TestResultsDirectory)
                    .SetLoggers("trx")
                    .SetVerbosity(DotNetVerbosity.normal));
            }
        });

    Target Pack => _ => _
        .Description("بسته‌بندی کتابخانه‌ها")
        .DependsOn(Test)
        .Executes(() =>
        {
            // پیدا کردن پروژه‌هایی که باید پکیج شوند (به جز پروژه‌های تست)
            var packableProjects = Solution.GetAllProjects("*")
                .Where(p => !p.Name.EndsWith(".Tests") && !p.Name.EndsWith(".Console"));

            // پکیج کردن هر پروژه
            foreach (var project in packableProjects)
            {
                DotNetPack(_ => _
                    .SetProject(project)
                    .SetConfiguration(Configuration)
                    .EnableNoBuild()
                    .EnableNoRestore()
                    .SetVersion(GitVersion.NuGetVersionV2)
                    .SetOutputDirectory(PackagesDirectory));
            }
        });

    // Target CopyPackagesToSolutionFolder => _ => _
    //     .Description("انتقال بسته‌ها به پوشه packages در مسیر Solution")
    //     .DependsOn(Pack)
    //     .Executes(() =>
    //     {
    //         // پوشه packages در مسیر Solution
    //         var solutionPackagesDir = Solution.Directory / "packages";
    //         EnsureExistingDirectory(solutionPackagesDir);
    //         
    //         // کپی کردن تمام پکیج‌ها به پوشه packages سولوشن
    //         CopyDirectoryRecursively(PackagesDirectory, solutionPackagesDir, DirectoryExistsPolicy.Merge);
    //         
    //         Console.WriteLine($"بسته‌ها با موفقیت به {solutionPackagesDir} منتقل شدند.");
    //     });

    Target PublishToNuGet => _ => _
        .Description("انتشار بسته‌ها در NuGet")
        // .DependsOn(CopyPackagesToSolutionFolder)
        .Requires(() => Configuration.Equals(Configuration.Release))
        .Requires(() => !string.IsNullOrWhiteSpace(NuGetApiKey))
        .Executes(() =>
        {
            // پیدا کردن تمام فایل‌های nupkg
            var packages = PackagesDirectory.GlobFiles("*.nupkg");

            // ارسال هر پکیج به NuGet
            foreach (var package in packages)
            {
                DotNetNuGetPush(_ => _
                    .SetTargetPath(package)
                    .SetApiKey(NuGetApiKey)
                    .SetSource("https://api.nuget.org/v3/index.json")
                    .SetSkipDuplicate(true));
            }

            Console.WriteLine($"تعداد {packages.Count} بسته با موفقیت در NuGet منتشر شد.");
        });

    Target CommitChanges => _ => _
        .Description("ثبت تغییرات در Git")
        .Requires(() => !string.IsNullOrWhiteSpace(CommitMessage))
        .Executes(() =>
        {
            Console.WriteLine( GitTasks.GitPath);
            // بررسی وجود تغییرات
            var statusResult = GitTasks.Git("status --porcelain").FirstOrDefault();
            if (string.IsNullOrEmpty(statusResult.Text))
            {
                Console.WriteLine("هیچ تغییری برای commit وجود ندارد.");
                return;
            }
            GitTasks.Git("add .");
            Console.WriteLine($"Commit Message: \"{CommitMessage}\"");
            GitTasks.Git($"commit -m '{CommitMessage}'");
            GitTasks.Git("log --oneline");
            
            Console.WriteLine($"تغییرات با پیام \"{CommitMessage}\" کامیت شدند.");
        });

    Target PushToGitHub => _ => _
        .Description("ارسال تغییرات به GitHub")
        .DependsOn(CommitChanges)
        .Requires(() => !string.IsNullOrWhiteSpace(GitHubBranch))
        .Requires(() => !string.IsNullOrWhiteSpace(GitHubUsername))
        .Requires(() => !string.IsNullOrWhiteSpace(GitHubToken))
        .Executes(() =>
        {
            // تنظیم نام کاربری و توکن برای دسترسی به GitHub
            var gitHubUrl =
                $"https://{GitHubUsername}:{GitHubToken}@github.com/{GitHubUsername}/{GitRepository.GetGitHubName()}.git";

            // اضافه کردن remote (اگر قبلاً وجود نداشته باشد)
            try
            {
                GitTasks.Git($"remote add github {gitHubUrl}");
            }
            catch
            {
                GitTasks.Git($"remote set-url github {gitHubUrl}");
            }

            // ارسال به شاخه مشخص شده
            GitTasks.Git($"push github HEAD:{GitHubBranch}");

            Console.WriteLine($"تغییرات با موفقیت به شاخه {GitHubBranch} در GitHub ارسال شدند.");
        });

    Target CompleteWorkflow => _ => _
        .Description("اجرای کامل جریان کاری: Build، Test، Pack، انتشار در NuGet و ارسال به GitHub")
        .DependsOn(PublishToNuGet, PushToGitHub)
        .Executes(() =>
        {
            Console.WriteLine("جریان کاری کامل با موفقیت انجام شد!");
        });
}