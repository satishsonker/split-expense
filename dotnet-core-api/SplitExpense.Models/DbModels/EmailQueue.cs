using SplitExpense.SharedResource;

namespace SplitExpense.Models.DbModels
{
    public class EmailQueue : BaseDbModels
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public EmailStatus Status { get; set; } = EmailStatus.Pending;
        public int RetryCount { get; set; } = 0;
        public DateTime? SentAt { get; set; }
    }
}
