namespace SplitExpense.ExceptionManagement.Exceptions
{
    public class UnauthorizedException(string message = "You are not authorized!") : Exception(message)
    {
    }
}