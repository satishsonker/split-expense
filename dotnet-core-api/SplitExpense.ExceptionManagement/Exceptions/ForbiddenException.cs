namespace SplitExpense.ExceptionManagement.Exceptions
{
    public class ForbiddenException(string message = "You are forbidden to access this resource!") : Exception(message)
    {
    }
}