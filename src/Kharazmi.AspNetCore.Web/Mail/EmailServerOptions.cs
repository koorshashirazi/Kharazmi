namespace Kharazmi.AspNetCore.Web.Mail
{
    /// <summary> </summary>
    public class EmailServerOptions
    {
        /// <summary> </summary>
        public string SmtpServer { get; set; }
        /// <summary> </summary>
        public int SmtpPort { get; set; }
        /// <summary> </summary>
        public string SmtpUsername { get; set; }
        /// <summary> </summary>
        public string SmtpPassword { get; set; }
        /// <summary> </summary>
        public string PopServer { get; set; }
        /// <summary> </summary>
        public int PopPort { get; set; }
        /// <summary> </summary>
        public string PopUsername { get; set; }
        /// <summary> </summary>
        public string PopPassword { get; set; }
        /// <summary> </summary>
        public string LocalDomain { get; set; }
        /// <summary> </summary>
        public bool UsePickupFolder { get; set; }
        /// <summary> </summary>
        public string PickupFolder { get; set; }
    }
}