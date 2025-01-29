namespace SplitExpense.Models.Common
{
    public class ExceptionResponse
    {
        public int StatusCode { get; set; }
        public ErrorResponse ErrorResponse { get; set; }
    }
}
