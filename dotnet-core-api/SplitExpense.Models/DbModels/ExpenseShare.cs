using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SplitExpense.Models
{
    public class ExpenseShare : BaseDbModels
    {
        [ForeignKey("Expense")]
        public int ExpenseId { get; set; }
        public virtual Expense Expense { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int SplitTypeId { get; set; }  // Store the type of split used

        public decimal? Percentage { get; set; }  // For Percentage-based splitting
        public decimal? Shares { get; set; }      // For Share-based splitting
        public decimal? ExactAmount { get; set; } // For Exact Amount-based splitting
        public decimal? AdjustedAmount { get; set; } // For Adjustment-based settlements

        [Required]
        public decimal AmountOwed { get; set; }   // Final calculated amount owed
    }
}
