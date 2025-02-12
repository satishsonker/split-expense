using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SplitExpense.Models.DbModels
{
    public class ExpenseShare : BaseDbModels
    {
        public int SplitTypeId { get; set; }  // Store the type of split used
        public decimal? Percentage { get; set; }  // For Percentage-based splitting
        public decimal? Shares { get; set; }      // For Share-based splitting
        public decimal? ExactAmount { get; set; } // For Exact Amount-based splitting
        public decimal? AdjustedAmount { get; set; } // For Adjustment-based settlements

        [Required]
        public decimal AmountOwed { get; set; }   // Final calculated amount owed
        public int ExpenseId { get; set; }
        [ForeignKey(nameof(ExpenseId))]
        public virtual Expense Expense { get; set; }

        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
    }
}
