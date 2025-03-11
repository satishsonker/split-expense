namespace SplitExpense.Models.DTO
{
    public class ExpenseActivityRequest
    {
        public int ActivityType { get; set; }
        public string Activity { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Icon { get; set; } = string.Empty;
    }
} 