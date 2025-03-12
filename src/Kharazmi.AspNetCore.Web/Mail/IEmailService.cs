using System.Collections.Generic;
using System.Threading.Tasks;
using MimeKit;

namespace Kharazmi.AspNetCore.Web.Mail
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void Send(MimeMessage message);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendAsync(MimeMessage message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        List<EmailMessage> ReceiveEmail(int maxCount = 10);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        Task<List<EmailMessage>> ReceiveEmailAsync(int maxCount = 10);
    }
}
