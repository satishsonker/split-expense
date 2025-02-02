using SplitExpense.SharedResource;

namespace SplitExpense.Models
{
    public class EmailQueue : BaseDbModels
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public EmailStatus Status { get; set; } = EmailStatus.Pending;
        public int RetryCount { get; set; } = 0;
        public int SmtpSettingsId { get; set; }
        public SmtpSettings SmtpSettings { get; set; }
        public DateTime? SentAt { get; set; }
    }
}
