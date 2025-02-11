namespace SplitExpense.Models.DTO
{
    public class ContactResponse
    {
        public required int UserId { get; set; }
        public required int ContactId { get; set; }

        public UserResponse ContactUser { get; set; }
    }
}
