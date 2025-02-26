using SplitExpense.Models.DbModels;
using SplitExpense.Models.DTO;

namespace SplitExpense.Data.Factory
{
    public interface IAuthFactory
    {
        Task<User?> ValidateUserAsync(string email, string password);
        Task<bool> UpdatePasswordAsync(int userId, string newPassword);
        Task<bool> ValidateResetTokenAsync(string token);
        Task<User?> GetUserByEmailOrPhoneAsync(string email,string phone);
        Task<bool> SaveResetTokenAsync(int userId, string token, DateTime expiry);
        Task<bool> UpdateUserLastLoginAsync(int userId);
        Task<User?> UpdateUserAsync(User user);
        Task<User?> UpdateProfilePictureAsync(int userId, FileUploadResponse fileUpload);
        Task<bool> DeleteProfilePictureAsync(int userId);
    }
}