namespace SplitExpense.Models.DTO
{
    public class DashboardSummaryResponse
    {
        public decimal TotalBalance { get; set; }
        public decimal YouOwe { get; set; }
        public decimal YouAreOwed { get; set; }
    }
}

