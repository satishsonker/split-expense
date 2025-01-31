namespace SplitExpense.EmailManagement.Models
{
    public class EmailTemplate : BaseDbModel
    {
        public required string Subject { get; set; }
        public required string Body { get; set; }
    }
}
