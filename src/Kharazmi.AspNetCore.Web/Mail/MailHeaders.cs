namespace Kharazmi.AspNetCore.Web.Mail
{
    /// <summary>
    /// Custom mail headers
    /// </summary>
    public class MailHeaders
    {
        /// <summary> </summary>
        public string MessageId { set; get; }

        /// <summary> </summary>
        public string InReplyTo { set; get; }

        /// <summary> </summary>
        public string References { set; get; }
    }
}