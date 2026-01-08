namespace SplitExpense.Models.DTO
{
    public class SettleAmountRequest
    {
        public int ToUserId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}

