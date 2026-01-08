using SplitExpense.Models.Base;

namespace SplitExpense.Models.DTO
{
    public class ExpenseShareResponse: BaseResponseModel
    {
        public int SplitTypeId { get; set; }
        public decimal? Percentage { get; set; }
        public decimal? Shares { get; set; }
        public decimal? ExactAmount { get; set; }
        public decimal? AdjustedAmount { get; set; }
        public decimal AmountOwed { get; set; }
        public int ExpenseId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
    }
}
