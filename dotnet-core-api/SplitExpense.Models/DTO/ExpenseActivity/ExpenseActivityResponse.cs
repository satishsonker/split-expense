namespace SplitExpense.Models.DTO
{
    public class ExpenseActivityResponse
    {
        public int Id { get; set; }
        public int ActivityType { get; set; }
        public string Activity { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 