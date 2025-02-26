using SplitExpense.Models.DbModels;
namespace SplitExpense.Data.Factory
{
    public interface IUserFactory
    {
        Task<User> CreateAsync(User user);
        Task<User?> GetUserByResetTokenAsync(string token);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<User?> GetByIdAsync(int id);
    }
}
