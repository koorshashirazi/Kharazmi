using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Localization.Json.Internal
{
    public static class Helpers
    {
        public static string GetResourcesPath(this Type resourceSource , string resourcesDir)
        {
            var assembly = resourceSource.GetAssembly();
            var binPath = Path.GetDirectoryName(assembly.Location);
            var rootPath = binPath?.Substring(0,
                binPath.IndexOf(assembly.GetName().Name, StringComparison.Ordinal) + assembly.GetName().Name.Length);
            var resPath = assembly.GetResourcePath(resourcesDir);
            var resourcesPath = Path.Combine(rootPath, resPath);
            return resourcesPath;
        }
        
        public static string GetResourcesPath(this Assembly assembly , string resourcesDir)
        {
            var assemblyName = assembly.GetName().Name;
            var binPath = Path.GetDirectoryName(assembly.Location);
            var rootPath = binPath?.Substring(0,
                binPath.IndexOf(assemblyName, StringComparison.Ordinal) + assemblyName.Length);
            var resPath = assembly.GetResourcePath(resourcesDir);
            var resourcesPath = Path.Combine(rootPath, resPath);
            return resourcesPath;
        }

        public static List<LocalizationEntity> ReadResources(string resourceName, Assembly resourceAssembly,
            CultureInfo cultureInfo,
            ILogger logger, bool isFallback)
        {
            Assembly satelliteAssembly;
            try
            {
                satelliteAssembly = resourceAssembly.GetSatelliteAssembly(cultureInfo);
            }
            catch (FileNotFoundException x)
            {
                logger.LogInformation(
                    $"Could not find satellite assembly for {(isFallback ? "fallback " : "")}'{cultureInfo.Name}': {x.Message}");
                return new List<LocalizationEntity>();
            }

            var stream = satelliteAssembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                logger.LogInformation(
                    $"Resource '{resourceName}' not found for {(isFallback ? "fallback " : "")}'{cultureInfo.Name}'.");
                return new List<LocalizationEntity>();
            }

            string json;
            using (var reader = new StreamReader(stream))
            {
                json = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<List<LocalizationEntity>>(json);
        }

        public static void WriteToResources(string resourceName, Assembly resourceAssembly, CultureInfo cultureInfo,
            ILogger logger, List<LocalizationEntity> jsonObject)
        {
            Assembly satelliteAssembly;
            try
            {
                satelliteAssembly = resourceAssembly.GetSatelliteAssembly(cultureInfo);
            }
            catch (FileNotFoundException x)
            {
                logger.LogInformation(
                    $"Could not find satellite assembly for '{cultureInfo.Name}': {x.Message}");
                return;
            }

            var stream = satelliteAssembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                logger.LogInformation(
                    $"Resource '{resourceName}' not found for '{cultureInfo.Name}'.");
                return;
            }

            var output = JsonConvert.SerializeObject(jsonObject);
            using var writer = new StreamWriter(stream);
            writer.WriteLine(output);
        }

        public static string GetComputedCacheKey(string resourceName, string resourceKey, string culture)
        {
            return string.Format(DefaultConfiguration.LocalizationCacheKeyTemplate, culture, resourceName, resourceKey);
        }
        
        public static string GetComputedCacheKey(this Type resourceSource, string resourceKey)
        {
            return string.Format(DefaultConfiguration.LocalizationCacheKeyTemplate, CultureInfo.CurrentUICulture.Name, resourceSource.GetTypeInfo().FullName, resourceKey);
        }
        
        public static string GetComputedCacheKey(this Type resourceSource, string resourceKey, string culture)
        {
            return string.Format(DefaultConfiguration.LocalizationCacheKeyTemplate, culture, resourceSource.GetTypeInfo().FullName, resourceKey);
        }

        public static string GetComputedResourceFile(string resourcesPath, string resourceName, string culture)
        {
            var resourceFile = resourceName.IsEmpty()
                ? $"{culture}.json"
                : $"{resourceName}.{culture}.json";

            var searchedLocation = Path.Combine(resourcesPath, resourceFile);

            if (!File.Exists(searchedLocation))
            {
                if (resourceFile.Count(r => r == '.') > 1)
                {
                    var resourceFileWithoutExtension = Path.GetFileNameWithoutExtension(resourceFile);
                    var resourceFileWithoutCulture =
                        resourceFileWithoutExtension.Substring(0, resourceFileWithoutExtension.LastIndexOf('.'));
                    resourceFile =
                        $"{resourceFileWithoutCulture.Replace('.', Path.DirectorySeparatorChar)}.{culture}.json";
                    searchedLocation = Path.Combine(resourcesPath, resourceFile);
                }
            }

            return searchedLocation;
        }

        public static string GetApplicationRoot()
            => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);


        public static string GetTypeName(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
//            var assemblyName = type.Assembly.GetName().Name;
//            var typeName = $"{assemblyName}.{typeInfo.Name}" == typeInfo.FullName
//                ? typeInfo.Name
//                : typeInfo.FullName?.Substring(assemblyName.Length + 1);

            return typeInfo.Name;
        }

        public static Assembly GetAssembly(this Type type)
        {
            return type.GetTypeInfo().Assembly;
        }

        public static string GetAssemblyFullName(this Type type)
        {
            return type.GetTypeInfo().Assembly.FullName;
        }

        public static Assembly GetAssemblyFromLocation(string location)
        {
            var assemblyName = new AssemblyName(location);
            var assembly = Assembly.Load(assemblyName);

            return assembly;
        }

        public static string GetResourcePath(this Assembly assembly, string resourceFolderName = "")
        {
            var resourceLocationAttribute = assembly.GetCustomAttribute<ResourceLocationAttribute>();

            return resourceLocationAttribute == null
                ? resourceFolderName
                : resourceLocationAttribute.ResourceLocation;
        }

        public static string TrimPrefix(string name, string prefix)
        {
            if (name.StartsWith(prefix, StringComparison.Ordinal))
            {
                return name.Substring(prefix.Length);
            }

            return name;
        }

        public static string TryFixInnerClassPath(string path)
        {
            var fixedPath = path;

            if (path.Contains("+"))
            {
                fixedPath = path.Replace('+', '.');
            }

            return fixedPath;
        }
    }
}