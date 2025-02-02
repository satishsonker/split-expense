using Microsoft.Extensions.Logging;
using SplitExpense.Data;
using SplitExpense.Models;
using SplitExpense.Logger;
using System;
using SplitExpense.SharedResource;
using Microsoft.EntityFrameworkCore;

namespace SplitExpense.EmailManagement.Service
{
    public class EmailQueueService(SplitExpenseDbContext context, ISplitExpenseLogger logger) : IEmailQueueService
    {
        private readonly SplitExpenseDbContext _context = context;
        private readonly ISplitExpenseLogger _logger = logger;

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

        public async Task ProcessQueue()
        {
            var pendingEmails = await _context.EmailQueues
                .Include(e => e.SmtpSettings)
                .Where(e => e.Status == EmailStatus.Pending)
                .ToListAsync();

            foreach (var email in pendingEmails)
            {
                try
                {
                   // await SendEmail(email);
                    email.Status = EmailStatus.Sent;
                    email.SentAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    email.RetryCount++;
                    if (email.RetryCount >= 3) email.Status = EmailStatus.Failed;
                    _logger.LogError(ex, $"Failed to send email {email.Id}");
                }
                await _context.SaveChangesAsync();
            }
        }

        public Task RetryFailedEmails(int maxRetries)
        {
            throw new NotImplementedException();
        }
    }
}
