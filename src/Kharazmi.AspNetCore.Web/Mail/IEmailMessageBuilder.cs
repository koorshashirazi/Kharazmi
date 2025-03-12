using System.Collections.Generic;
using MimeKit;

namespace Kharazmi.AspNetCore.Web.Mail
{
    public interface IEmailMessageBuilder
    {
        IEmailMessageBuilder WithHeader(MailHeaders headers , string fromAddress);
        IEmailMessageBuilder WithAttachmentFiles(List<string> pathFiles);
        IEmailMessageBuilder WithReplyTos(IReadOnlyCollection<MailAddress> mailAddress);
        IEmailMessageBuilder WithBcc(IReadOnlyCollection<MailAddress> mailAddress);
        IEmailMessageBuilder WithCc(IReadOnlyCollection<MailAddress> mailAddress);
        IEmailMessageBuilder WithSender(MailAddress mailAddress);
        IEmailMessageBuilder WithReceiver(MailAddress mailAddress);
        IEmailMessageBuilder WithSubject(string subject);
        IEmailMessageBuilder WithSubject(string template, params object[] @params);
        IEmailMessageBuilder WithHtmlBody(string body);
        IEmailMessageBuilder WithStringBody(string template, params object[] @params);
        MimeMessage Build();
    }
}
