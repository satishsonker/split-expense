using Microsoft.AspNetCore.Http;
using SplitExpense.Models.DTO;
using SplitExpense.Models.DTO.Response;

namespace SplitExpense.Logic
{
    public interface IAuthLogic
    {
        Task<LoginResponse> CreateUserAsync(CreateUserRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<bool> LogoutAsync(int userId);
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request);
        Task<bool> ForgotUsernameAsync(ForgotUsernameRequest request);
        Task<UserResponse> UpdateUserAsync(UpdateUserRequest request);
        Task<UserResponse> UpdateProfilePictureAsync(int userId, IFormFile file);
        Task<bool> DeleteProfilePictureAsync(int userId);
        Task<bool> DeleteUserAsync(int userId);
    }
}