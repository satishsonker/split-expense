namespace SplitExpense.Models.DTO
{
    public class MemberBalanceResponse
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? ProfilePicture { get; set; }
        public decimal YouOwe { get; set; } // Amount you owe to this member
        public decimal YouAreOwed { get; set; } // Amount this member owes you
        public decimal NetBalance { get; set; } // YouAreOwed - YouOwe (positive = they owe you, negative = you owe them)
    }
}

