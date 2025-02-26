namespace SplitExpense.Models.DTO
{
    public class UpdateUserRequest
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? LanguageCode { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CountryCode { get; set; }
        public string? ISDCode { get; set; }
        public string? Timezone { get; set; }
    }
} 