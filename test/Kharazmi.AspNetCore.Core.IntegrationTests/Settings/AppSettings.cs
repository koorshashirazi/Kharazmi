using Kharazmi.AspNetCore.Core.Configuration;
using Kharazmi.MessageBroker;
using Microsoft.AspNetCore.Builder;

namespace Kharazmi.AspNetCore.Core.IntegrationTests.Settings
{
    public class AppSettings : AppSetting
    {
        
        public int DefaultProcessVersion { get; set; }
        public string DefaultProcess { get; set; }
        public string[] ProcessesSupported { get; set; }
        public Logging Logging { get; set; }
        public LocalizationAppOptions LocalizationAppOptions { get; set; }
        public SessionOptions SessionOptions { get; set; }
        public HostInfoOptions HostInfoOptions { get; set; }
        public PaginationOptions PaginationOptions { get; set; }

        public EmailOptions EmailOptions { get; set; }
        public RabbitMqOptions RabbitMqOptions { get; set; }

        public SinglePageOption SinglePageOption { get; set; }
        public DomainHandlerRetryOptions DomainHandlerRetryOptions { get; set; }
        public override AppSetting Update(AppSetting appSetting)
        {
            var current = appSetting as AppSettings;
            if (current == null) return appSetting;
            UpdateOnLoad = current.UpdateOnLoad;
            DefaultProcessVersion = current.DefaultProcessVersion;
            DefaultProcess = current.DefaultProcess;
            ProcessesSupported = current.ProcessesSupported;
            Logging = current.Logging;
            HostInfoOptions = current.HostInfoOptions;
            PaginationOptions = current.PaginationOptions;
            EmailOptions = current.EmailOptions;
            RabbitMqOptions = current.RabbitMqOptions;
            SinglePageOption = current.SinglePageOption;
            DomainHandlerRetryOptions = current.DomainHandlerRetryOptions;
            return current;
        }
    }
}