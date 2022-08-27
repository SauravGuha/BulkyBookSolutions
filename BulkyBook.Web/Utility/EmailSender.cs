using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace BulkyBook.Web.Utility
{
    public class BulkyBookEmailSender : IEmailSender
    {
        private IConfiguration _configuration;

        public BulkyBookEmailSender(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var senderEmailId = _configuration.GetValue<string>("SenderMailId");
            var smtpHost = _configuration.GetValue<string>("SmtpHost");
            var smtpPort = _configuration.GetValue<string>("SmtpPort");
            var smtpUserId = _configuration.GetValue<string>("SmtpUser");
            var smtpPassword = _configuration.GetValue<string>("SmtpPassword");

            var mail = new MimeMessage();
            mail.From.Add(MailboxAddress.Parse(senderEmailId));
            mail.To.Add(MailboxAddress.Parse(email));
            mail.Subject = subject;
            mail.Body = new TextPart(htmlMessage);

            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    await smtpClient.ConnectAsync(smtpHost, int.Parse(smtpPort), MailKit.Security.SecureSocketOptions.StartTls);
                    smtpClient.Authenticate(smtpUserId, smtpPassword);
                    await smtpClient.SendAsync(mail);
                    await smtpClient.DisconnectAsync(true);
                }
            }
            catch { }
        }
    }
}
