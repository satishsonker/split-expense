using SplitExpense.Models;
using SplitExpense.Models.DTO.Response;

namespace SplitExpense.Logic
{
    public interface IEmailLogic
    {
        Task SendEmailOnUserAddedInGroup(string toEmail, string addedByUserName, string addedUserName,DateTime addedOn,Dictionary<string,string>? data);
        Task<EmailQueueResponse> GetEmailQueueAsync(PagingRequest pagingRequest);
        Task<bool> SendEmailOnPasswordResetAsync(string toEmail, string userName, DateTime requestTime, Dictionary<string, string> emailData);
        Task<bool> SendEmailOnPasswordResetSuccessAsync(string toEmail, string userName, DateTime requestTime, Dictionary<string, string> emailData);
        Task<bool> SendEmailOnUsernameReminderAsync(string toEmail, string userName, DateTime requestTime, Dictionary<string, string> emailData);
    }
}
