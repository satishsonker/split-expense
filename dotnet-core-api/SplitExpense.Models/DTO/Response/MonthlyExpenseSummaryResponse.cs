namespace SplitExpense.Models.DTO
{
    public class MonthlyExpenseSummaryResponse
    {
        public string Month { get; set; } // Format: "YYYY-MM" or "Jan 2024"
        public int Year { get; set; }
        public int MonthNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public int ExpenseCount { get; set; }
        public decimal YouOwe { get; set; }
        public decimal YouAreOwed { get; set; }
        public decimal NetBalance { get; set; }
    }
}

