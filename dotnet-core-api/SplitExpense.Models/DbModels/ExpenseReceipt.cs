using SplitExpense.SharedResource;

namespace SplitExpense.Models.DbModels
{
    public class ExpenseReceipt : BaseDbModels
    {
        public string ReceiptPath { get; set; }
        public string? ReceiptThumbPath { get; set; }
        public int ExpenseId { get; set; }
        public ReceiptTypes ReceiptType { get; set; }
    }
}
