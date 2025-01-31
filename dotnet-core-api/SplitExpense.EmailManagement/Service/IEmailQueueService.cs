using SplitExpense.EmailManagement.Models;

namespace SplitExpense.EmailManagement.Service
{
    public interface IEmailQueueService
    {
        Task AddEmailToQueue(string toEmail, string subject, string body, int smtpSettingsId);
        Task ProcessQueue();
        Task RetryFailedEmails(int maxRetries);
        Task<List<EmailQueue>> GetQueueByStatus(EmailStatus status);
        Task<List<EmailQueue>> FilterEmails(DateTime startDate, DateTime endDate);
        Task ManualTrigger();
        Task ManualSendEmail(int emailId);
    }
}
