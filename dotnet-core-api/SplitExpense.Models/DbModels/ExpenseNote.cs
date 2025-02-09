using System.ComponentModel.DataAnnotations.Schema;

namespace SplitExpense.Models
{
    public class ExpenseNote:BaseDbModels
    {
        public int Note { get; set; }
        public int ExpenseId { get; set; }

        [ForeignKey(nameof(ExpenseId))]
        public Expense? Expense { get; set; }
    }
}
