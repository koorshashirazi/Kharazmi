using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Kharazmi.AspNetCore.Core.Threading;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MimeKit;

namespace Kharazmi.AspNetCore.Web.Mail
{
    /// <summary> Email server service </summary>
    internal class EmailService : IEmailService
    {
        private readonly EmailServerOptions _serverOptions;

        /// <summary> </summary>
        /// <param name="serverOptions"></param>
        public EmailService(EmailServerOptions serverOptions)
        {
            _serverOptions = Ensure.ArgumentIsNotNull(serverOptions, nameof(serverOptions));
        }

        /// <summary></summary>
        /// <param name="message"></param>
        public void Send(MimeMessage message)
        {
            AsyncHelper.RunSync(() => SendAsync(message));
        }


        /// <summary>
        /// Sends an email using the `MailKit` library.
        /// </summary>
        public async Task SendAsync(MimeMessage message)
        {
            if (_serverOptions.UsePickupFolder)
            {
                const int maxBufferSize = 0x10000; // 64K.

                using var stream = new FileStream(
                    Path.Combine(_serverOptions.PickupFolder, $"email-{Guid.NewGuid():N}.eml"),
                    FileMode.CreateNew, FileAccess.Write, FileShare.None,
                    maxBufferSize, true);

                await message.WriteToAsync(stream).ConfigureAwait(false);
            }
            else
            {
                using var client = new SmtpClient();
                if (!string.IsNullOrWhiteSpace(_serverOptions.LocalDomain))
                {
                    client.LocalDomain = _serverOptions.LocalDomain;
                }

                await client.ConnectAsync(_serverOptions.SmtpServer, _serverOptions.SmtpPort).ConfigureAwait(false);

                if (_serverOptions.SmtpUsername.IsNotEmpty() && _serverOptions.SmtpPassword.IsNotEmpty())
                {
                    await client.AuthenticateAsync(_serverOptions.SmtpUsername, _serverOptions.SmtpPassword).ConfigureAwait(false);
                }

                await client.SendAsync(message).ConfigureAwait(false);

                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }

        /// <summary> </summary>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public List<EmailMessage> ReceiveEmail(int maxCount = 10)
        {
            return AsyncHelper.RunSync(() => ReceiveEmailAsync(maxCount));
        }

        /// <summary> </summary>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public async Task<List<EmailMessage>> ReceiveEmailAsync(int maxCount = 10)
        {
            using var emailClient = new Pop3Client();
            await emailClient.ConnectAsync(_serverOptions.PopServer, _serverOptions.PopPort, true).ConfigureAwait(false);

            emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

            if (_serverOptions.PopUsername.IsNotEmpty() && _serverOptions.PopPassword.IsNotEmpty())
                await emailClient.AuthenticateAsync(_serverOptions.PopUsername, _serverOptions.PopPassword).ConfigureAwait(false);

            var emails = new List<EmailMessage>();
            for (var i = 0; i < emailClient.Count && i < maxCount; i++)
            {
                var message = emailClient.GetMessage(i);
                var emailMessage = new EmailMessage
                {
                    Content = message.HtmlBody.IsNotEmpty() ? message.HtmlBody : message.TextBody,
                    Subject = message.Subject
                };
                emailMessage.Receivers.AddRange(message.To.Select(x => (MailboxAddress) x)
                    .Select(x => MailAddress.For(x.Name, x.Address)));
                emailMessage.Senders.AddRange(message.From.Select(x => (MailboxAddress) x)
                    .Select(x => MailAddress.For(x.Name, x.Address)));
                emails.Add(emailMessage);
            }

            return emails;
        }
    }
}