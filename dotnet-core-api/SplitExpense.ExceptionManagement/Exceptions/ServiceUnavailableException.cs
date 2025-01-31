namespace SplitExpense.ExceptionManagement.Exceptions
{
    public class ServiceUnavailableException(string healthReportStatus) : Exception
    {
        public string HealthReportStatus { get; set; } = healthReportStatus;
    }
}