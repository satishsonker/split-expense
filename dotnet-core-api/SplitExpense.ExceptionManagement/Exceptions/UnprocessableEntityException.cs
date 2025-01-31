namespace SplitExpense.ExceptionManagement.Exceptions
{
    public class UnprocessableEntityException(string message = "Unprocessable Entity!") : Exception(message)
    {
    }
}