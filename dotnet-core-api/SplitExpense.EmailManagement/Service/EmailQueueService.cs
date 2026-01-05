using Microsoft.EntityFrameworkCore;
using SplitExpense.Data;
using SplitExpense.Logger;
using SplitExpense.Models;
using SplitExpense.SharedResource;
using SplitExpense.ExceptionManagement.Exceptions;
using Microsoft.Extensions.Configuration;
using SplitExpense.Models.Common;
using SplitExpense.Models.DbModels;

namespace SplitExpense.EmailManagement.Service
{
    public class EmailQueueService(SplitExpenseDbContext context, ISplitExpenseLogger logger, IConfiguration configuration, EmailServiceFactory emailServiceFactory) : IEmailQueueService
    {
        private readonly SplitExpenseDbContext _context = context;
        private readonly ISplitExpenseLogger _logger = logger;
        private readonly IConfiguration _configuration = configuration;
        private readonly IEmailService _emailService=emailServiceFactory.GetEmailService();

        public async Task<bool> AddEmailToQueue(string toEmail, string subject, string body)
        {
            if (string.IsNullOrEmpty(toEmail) || string.IsNullOrWhiteSpace(toEmail))
            {
                _logger.LogError(null, LogMessage.InvalidToEmailAddress.ToFormattedString(), "EmailQueueService-AddEmailToQueue");
                throw new BusinessRuleViolationException(ErrorCodes.InvalidFieldFormat.ToString(), UserMessage.InvalidEmailAddress);
            }
            if (string.IsNullOrEmpty(subject) || string.IsNullOrWhiteSpace(subject))
            {
                _logger.LogError(null, LogMessage.InvalidEmailSubject.ToFormattedString(), "EmailQueueService-AddEmailToQueue");
                throw new BusinessRuleViolationException(ErrorCodes.InvalidFieldFormat.ToString(), UserMessage.InvalidEmailSubject);
            }
            if (string.IsNullOrEmpty(body) || string.IsNullOrWhiteSpace(body))
            {
                _logger.LogError(null, LogMessage.InvalidEmailBody.ToFormattedString(), "EmailQueueService-AddEmailToQueue");
                throw new BusinessRuleViolationException(ErrorCodes.InvalidFieldFormat.ToString(), UserMessage.InvalidEmailBody);
            }

            int retryCount = _configuration.GetSection("EmailSettings:SendEmailRetryCount").Get<int>();
            
            var newEmailQueue = new EmailQueue()
            {
                Body = body,
                CreatedAt = DateTime.Now,
                RetryCount = retryCount <= 0 ? 3 : retryCount,
                Status=EmailStatus.Pending,
                ToEmail = toEmail,
                Subject = subject                
            };

            await _context.EmailQueues.AddAsync(newEmailQueue);
            return await _context.SaveChangesAsync()>0;
        }

        public Task<PagingResponse<EmailQueue>> FilterEmails(PagingRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PagingResponse<EmailQueue>> GetQueueByStatus(PagingRequest request, EmailStatus status)
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
                .Where(e => e.Status == EmailStatus.Pending)
                .ToListAsync();

            foreach (var email in pendingEmails)
            {
                try
                {
                    if (await _emailService.SendEmailAsync(email.ToEmail,email.Subject,email.Body))
                    {
                        email.Status = EmailStatus.Sent;
                        email.SentAt = DateTime.UtcNow;
                        email.Status=EmailStatus.Sent;
                        email.UpdatedAt = DateTime.UtcNow;
                        email.UpdatedBy = 0;
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    email.RetryCount++;
                    if (email.RetryCount >= 3) email.Status = EmailStatus.Failed;
                    _logger.LogError(ex, $"Failed to send email {email.Id}");
                    await _context.SaveChangesAsync();
                }
            }
        }

        public Task RetryFailedEmails(int maxRetries)
        {
            throw new NotImplementedException();
        }

    }
}
