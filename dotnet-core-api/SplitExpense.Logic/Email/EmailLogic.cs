
using SplitExpense.EmailManagement.Service;
using SplitExpense.SharedResource;
using SplitExpense.ExceptionManagement.Exceptions;
using SplitExpense.Models.DTO.Response;
using SplitExpense.Models;
using AutoMapper;
using SplitExpense.Logger;

namespace SplitExpense.Logic.Email
{
    public class EmailLogic(IEmailTemplateService emailTemplateService,IEmailQueueService emailQueueService,IMapper mapper,ISplitExpenseLogger logger) : IEmailLogic
    {
        private readonly IEmailTemplateService _emailTemplateService = emailTemplateService;
        private readonly IEmailQueueService _emailQueueService = emailQueueService;
        private readonly IMapper _mapper=mapper;
        private readonly ISplitExpenseLogger _logger = logger;

        public async Task<EmailQueueResponse> GetEmailQueueAsync(PagingRequest pagingRequest)
        {
            return _mapper.Map<EmailQueueResponse>(await _emailQueueService.FilterEmails(pagingRequest));
        }

        public async Task SendEmailOnUserAddedInGroup(string toEmail, string addedByUserName, string addedUserName, DateTime addedOn, Dictionary<string, string>? data)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(data);

                var emailTemplate = await _emailTemplateService.GetTemplateByCodeAsync(EmailTemplateCode.UserAddedToExpenseGroup) ?? throw new BusinessRuleViolationException(ErrorCodes.NotFound);
                if (emailTemplate == null)
                {
                    _logger.LogError(null, LogMessage.NotFoundEmailTemplate, "EmailLogic-SendEmailOnUserAddedInGroup");
                }
                if (!data.TryGetValue("groupName",out string? groupName))
                    throw new KeyNotFoundException("groupName");
                
                if (!data.TryGetValue("addedByUserEmail", out string? addedByUserEmail)) 
                    throw new KeyNotFoundException("addedByUserEmail");

                emailTemplate.Subject = emailTemplate.Subject.Replace("##addedByUserName##", addedByUserName);
                emailTemplate.Subject = emailTemplate.Subject.Replace("##groupName##", groupName);

                emailTemplate.Body = emailTemplate.Body.Replace("##addedByUserName##", addedByUserName);
                emailTemplate.Body = emailTemplate.Body.Replace("##addedByUserEmail##", addedByUserEmail);
                emailTemplate.Body = emailTemplate.Body.Replace("##groupName##", data["groupName"]);
                await _emailQueueService.AddEmailToQueue(toEmail, emailTemplate.Subject, emailTemplate.Body);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex,LogMessage.NotFoundEmailTemplate, "EmailLogic-SendEmailOnUserAddedInGroup");
                throw;
            }
        }
    }
}
