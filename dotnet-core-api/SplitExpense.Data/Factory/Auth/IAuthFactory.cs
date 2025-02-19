using SplitExpense.Models.DbModels;

namespace SplitExpense.Data.Factory
{
    public interface IAuthFactory
    {
        Task<User?> ValidateUserAsync(string email, string password);
        Task<bool> UpdatePasswordAsync(int userId, string newPassword);
        Task<bool> ValidateResetTokenAsync(string token);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> SaveResetTokenAsync(int userId, string token, DateTime expiry);
        Task<bool> UpdateUserLastLoginAsync(int userId);
    }
}