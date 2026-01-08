namespace SplitExpense.Models.DTO
{
    public class GroupSummaryResponse
    {
        public int GroupId { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal YouOwe { get; set; }
        public decimal YouAreOwed { get; set; }
        public decimal YourBalance { get; set; } // YouAreOwed - YouOwe
        public List<GroupMemberBalanceResponse> MemberBalances { get; set; }
    }

    public class GroupMemberBalanceResponse
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? ProfilePicture { get; set; }
        public decimal Balance { get; set; } // Positive = they owe you, Negative = you owe them
    }
}

