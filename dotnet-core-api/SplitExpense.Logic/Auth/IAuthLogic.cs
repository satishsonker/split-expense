using SplitExpense.Models.DTO;

namespace SplitExpense.Logic
{
    public interface IAuthLogic
    {
        Task<LoginResponse> CreateUserAsync(CreateUserRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<bool> LogoutAsync(int userId);
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
        Task<bool> ForgotUsernameAsync(ForgotUsernameRequest request);
    }
}