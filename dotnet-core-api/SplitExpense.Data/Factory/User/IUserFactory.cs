using SplitExpense.Models.DbModels;
namespace SplitExpense.Data.Factory
{
    public interface IUserFactory
    {
        Task<User?> GetUserByResetTokenAsync(string resetToken);
        Task<User> CreateAsync(User user);
    }
}
