namespace SplitExpense.Middleware.Exceptions
{
    public class UnprocessableEntityException(string message = "Unprocessable Entity!") : Exception(message)
    {
    }
}