
using SplitExpense.EmailManagement.Service;

namespace SplitExpense.Logic.Email
{
    public class EmailLogic(IEmailTemplateService emailTemplateService) : IEmailLogic
    {
        IEmailTemplateService _emailTemplateService=emailTemplateService;
        public Task SendEmailOnUserAddedInGroup(string toEmail, string addedByUserName, string addedUserName, DateTime addedON)
        {
            //_emailTemplateService.
            throw new NotImplementedException();
        }
    }
}
