using SplitExpense.Models;
using SplitExpense.Models.DTO.Response;

namespace SplitExpense.Logic.Email
{
    public interface IEmailLogic
    {
        Task SendEmailOnUserAddedInGroup(string toEmail, string addedByUserName, string addedUserName,DateTime addedOn,Dictionary<string,string>? data);
        Task<EmailQueueResponse> GetEmailQueueAsync(PagingRequest pagingRequest);
    }
}
