using Kharazmi.AspNetCore.Web.Mail;

 namespace Kharazmi.AspNetCore.Core.Test.Settings
{
    public class EmailOptions : EmailServerOptions
    {
        public string Subject { get; set; }
        public string SenderName { get; set; }
        public string SenderAddress { get; set; }
    }
}