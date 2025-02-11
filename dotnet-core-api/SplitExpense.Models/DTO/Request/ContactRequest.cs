using SplitExpense.Models.Base;

namespace SplitExpense.Models.DTO
{
    public class ContactRequest:BaseRequestModel
    {
        public required int UserId { get; set; }
        public required int ContactId { get; set; }
    }
}
