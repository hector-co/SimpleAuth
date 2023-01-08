using NETCore.MailKit.Core;
using SimpleAuth.Application;

namespace SimpleAuth.Infrastructure
{
    public class EmailSender : IEmailSender
    {
        private readonly IEmailService _emailService;

        public EmailSender(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            await _emailService.SendAsync(email, subject, message, isHtml: true);
        }
    }
}
