using System.Collections.Generic;

 namespace Kharazmi.AspNetCore.Web.Mail
{
    /// <summary> </summary>
    public class EmailMessage
    {
         /// <summary> </summary>
        public EmailMessage()
        {
            Receivers = new List<MailAddress>();
            Senders = new List<MailAddress>();
        }

         /// <summary> </summary>
        public List<MailAddress> Receivers { get; set; }
         /// <summary> </summary>
        public List<MailAddress> Senders { get; set; }
         /// <summary> </summary>
        public string Subject { get; set; }
         /// <summary> </summary>
        public string Content { get; set; }
    }
}