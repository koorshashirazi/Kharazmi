using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kharazmi.AspNetCore.Core.Configuration
{
    public class AppSettingsConfigurationSource : JsonConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            Optional = false;
            ReloadOnChange = false;
            EnsureDefaults(builder);
            return new SettingsProvider(this);
        }
    }

    public class SettingsProvider : JsonConfigurationProvider
    {
        public SettingsProvider(JsonConfigurationSource source) : base(source)
        {
        }

        public void Reload()
        {
            Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var file = Source.FileProvider?.GetFileInfo(Source.Path);
            if (file != null && file.Exists)
            {
                using var stream = file.CreateReadStream();
                Load(stream);
            }

            OnReload();
        }
    }

    public abstract class AppSetting
    {
        public bool UpdateOnLoad { get; set; }
        public abstract AppSetting Update(AppSetting appSetting);
    }

    public interface ISettingsService<TAppSetting> where  TAppSetting :AppSetting
    {
        TAppSetting Current { get; }

        void Update(Action<TAppSetting> configure);
    }

    public class SettingsService<TAppSetting> : ISettingsService<TAppSetting> where TAppSetting : AppSetting
    {
        private readonly SettingsProvider _configurationProvider;
        private readonly IOptionsMonitor<TAppSetting> _optionsMonitor;
        private readonly object _syncLock = new object();

        public SettingsService(
            SettingsProvider configurationProvider,
            IOptionsMonitor<TAppSetting> optionsMonitor)
        {
            _configurationProvider = configurationProvider;
            _optionsMonitor = optionsMonitor;
        }

        public TAppSetting Current => _optionsMonitor.CurrentValue;

        public void Update(Action<TAppSetting> configure)
        {
            if (configure == null) return;
            var newOptions = Current.Update(Current);
            configure(newOptions as TAppSetting);

            lock (_syncLock)
            {
                // writing new settings (with some fault tolerance)
                var configSource = _configurationProvider.Source;
                var fileName = Path.Combine(((PhysicalFileProvider) configSource.FileProvider).Root, configSource.Path);
                var tempFileName = fileName + ".new";

                var jsonOptions = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Include,
                    MaxDepth = 5,
                    FloatFormatHandling = FloatFormatHandling.String,
                    FloatParseHandling = FloatParseHandling.Double,
                    MissingMemberHandling = MissingMemberHandling.Error,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Local,
                    DefaultValueHandling = DefaultValueHandling.Populate,
                    Converters = new List<JsonConverter>
                    {
                        new StringEnumConverter()
                        //new Newtonsoft.Json.Converters.IsoDateTimeConverter()
                    }
                };

                File.WriteAllText(tempFileName, JsonConvert.SerializeObject(newOptions, jsonOptions));
                if (File.Exists(fileName)) File.Delete(fileName);

                File.Move(tempFileName, fileName);

                // signalling change
                _configurationProvider.Reload();
            }
        }
    }
}