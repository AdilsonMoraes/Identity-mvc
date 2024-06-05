using Identity.Domain.Enums;
using Identity.Services.Log;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Identity.Services.Common
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogService _logService;

        public EmailSender(IConfiguration configuration, ILogService logService)
        {
            _configuration = configuration;
            _logService = logService;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            if (string.IsNullOrEmpty(_configuration["SendGridKey"]))
            {
                throw new Exception("Null SendGridKey");
            }
            Execute(_configuration["SendGridKey"], subject, message, toEmail);
        }

        public async Task Execute(string apiKey, string subject, string message, string toEmail)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_configuration["FromEmail"], _configuration["FromName"]),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(toEmail));

            var sendBcc = _configuration["EmailBcc"] ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(sendBcc))
                msg.AddBcc(new EmailAddress(_configuration["EmailBcc"]));

            msg.SetClickTracking(false, false);
            var response = await client.SendEmailAsync(msg);
            _logService.LogMessage(response.IsSuccessStatusCode ? $"Email to {toEmail} queued successfully!" : $"Failure Email to {toEmail}", Guid.NewGuid(), LogType.Debug);
        }

    }
}
