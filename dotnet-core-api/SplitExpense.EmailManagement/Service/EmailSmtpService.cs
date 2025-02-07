using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SplitExpense.Logger;
using SplitExpense.Models.ConfigModels;
namespace SplitExpense.EmailManagement.Service
{
    public class EmailSmtpService(IOptions<EmailSettings> options, ISplitExpenseLogger logger) : IEmailService
    {
        private readonly EmailSettings _emailSettings = options.Value;
        private readonly ISplitExpenseLogger _logger = logger;

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            var smtpKey = _emailSettings.DefaultSmtp;
            if (!_emailSettings.SmtpProviders.TryGetValue(smtpKey, out var smtpConfig))
            {
                _logger.LogError($"SMTP provider '{smtpKey}' not found in configuration.");
                return false;
            }

            try
            {
                using var smtp = new SmtpClient
                {
                    Host = smtpConfig.Host,
                    Port = smtpConfig.Port,
                    EnableSsl = smtpConfig.EnableSsl,
                    Credentials = new NetworkCredential(smtpConfig.Username, smtpConfig.Password)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpConfig.From),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(to);

                await smtp.SendMailAsync(mailMessage);
                _logger.LogInfo($"Email sent successfully to {to} via SMTP ({smtpKey}).");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email via SMTP ({smtpKey}): {ex.Message}");
                return false;
            }
        }
    }
}
