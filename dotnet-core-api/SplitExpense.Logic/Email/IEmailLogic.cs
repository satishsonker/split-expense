namespace SplitExpense.Logic.Email
{
    public interface IEmailLogic
    {
        Task SendEmailOnUserAddedInGroup(string toEmail, string addedByUserName, string addedUserName,DateTime addedON);
    }
}
