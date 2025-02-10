using SplitExpense.Models.Base;

namespace SplitExpense.Models.DTO
{
    public class UserResponse:BaseResponseModel
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? ProfilePicture { get; set; }
        public string? LanguageCode { get; set; } = "us-en";
        public string? CurrencyCode { get; set; } = "INR";
        public string? CountryCode { get; set; }
        public string? ISDCode { get; set; }
    }
}
