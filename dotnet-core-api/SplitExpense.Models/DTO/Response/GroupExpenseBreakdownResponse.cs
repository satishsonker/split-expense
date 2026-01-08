namespace SplitExpense.Models.DTO
{
    public class GroupExpenseBreakdownResponse
    {
        public int GroupId { get; set; }
        public decimal TotalSpending { get; set; }
        public List<MemberExpenseBreakdown> MemberBreakdown { get; set; }
    }

    public class MemberExpenseBreakdown
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? ProfilePicture { get; set; }
        public decimal TotalPaid { get; set; } // Total amount they paid
        public decimal TotalOwed { get; set; } // Total amount they owe
        public decimal NetAmount { get; set; } // Net amount (positive = they paid more, negative = they owe)
        public decimal Percentage { get; set; } // Percentage of total spending
    }
}

