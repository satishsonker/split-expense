namespace SplitExpense.Models.DTO
{
    public class LoginResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenExpiration { get; set; }
        public UserResponse User { get; set; } = null!;
    }
}