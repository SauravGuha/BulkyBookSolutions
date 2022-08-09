using Microsoft.AspNetCore.Identity.UI.Services;

namespace BulkyBook.Web.Utility
{
    public class BulkyBookEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Task.CompletedTask;
        }
    }
}
