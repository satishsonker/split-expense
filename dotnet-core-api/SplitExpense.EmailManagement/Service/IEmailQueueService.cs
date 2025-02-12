using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DbModels;
using SplitExpense.SharedResource;

namespace SplitExpense.EmailManagement.Service
{
    public interface IEmailQueueService
    {
        Task<bool> AddEmailToQueue(string toEmail, string subject, string body);
        Task ProcessQueue();
        Task RetryFailedEmails(int maxRetries);
        Task<PagingResponse<EmailQueue>> GetQueueByStatus(PagingRequest request, EmailStatus status);
        Task<PagingResponse<EmailQueue>> FilterEmails(PagingRequest request);
        Task ManualTrigger();
        Task ManualSendEmail(int emailId);
    }
}
