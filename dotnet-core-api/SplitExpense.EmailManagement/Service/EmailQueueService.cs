using SplitExpense.EmailManagement.Models;

namespace SplitExpense.EmailManagement.Service
{
    public class EmailQueueService : IEmailQueueService
    {
        public Task AddEmailToQueue(string toEmail, string subject, string body, int smtpSettingsId)
        {
            throw new NotImplementedException();
        }

        public Task<List<EmailQueue>> FilterEmails(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<List<EmailQueue>> GetQueueByStatus(EmailStatus status)
        {
            throw new NotImplementedException();
        }

        public Task ManualSendEmail(int emailId)
        {
            throw new NotImplementedException();
        }

        public Task ManualTrigger()
        {
            throw new NotImplementedException();
        }

        public Task ProcessQueue()
        {
            throw new NotImplementedException();
        }

        public Task RetryFailedEmails(int maxRetries)
        {
            throw new NotImplementedException();
        }
    }
}
