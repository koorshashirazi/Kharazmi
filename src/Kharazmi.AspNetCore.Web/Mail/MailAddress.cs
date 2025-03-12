namespace Kharazmi.AspNetCore.Web.Mail
{
    /// <summary>
    /// Defines an email address
    /// </summary>
    public class MailAddress
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        public MailAddress(string name, string email)
        {
            Name = name;
            Email = email;
        }

        /// <summary> </summary>
        public string Name { set; get; }

        /// <summary> </summary>
        public string Email { set; get; }

        public static MailAddress For(string name, string email) => new MailAddress(name, email);
    }
}