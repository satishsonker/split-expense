using SplitExpense.Models.Base;

namespace SplitExpense.Models.DTO
{
    public class ExpenseShareRequest : BaseRequestModel
    {
        public int SplitTypeId { get; set; }  // Store the type of split used
        public decimal? Percentage { get; set; }  // For Percentage-based splitting
        public decimal? Shares { get; set; }      // For Share-based splitting
        public decimal? ExactAmount { get; set; } // For Exact Amount-based splitting
        public decimal? AdjustedAmount { get; set; }
        public decimal AmountOwed { get; set; }
        public int UserId { get; set; }  // User who owes this share
    }
}
