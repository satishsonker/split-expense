using Microsoft.AspNetCore.Identity;

namespace SplitExpense.Services
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUserAsync(string username, string email, string password);
        Task<string> LoginUserAsync(string username, string password);
        Task<IdentityResult> DeleteUserAsync(string username);
        Task<string> GeneratePasswordResetTokenAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword);
        Task<IdentityResult> ChangePasswordAsync(string username, string currentPassword, string newPassword);
    }

}
