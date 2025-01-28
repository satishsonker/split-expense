namespace SplitExpense.Middleware.Exceptions
{
    public class UnauthorizedException(string message = "You are not authorized!") : Exception(message)
    {
    }
}