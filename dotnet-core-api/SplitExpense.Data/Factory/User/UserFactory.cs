using Microsoft.EntityFrameworkCore;
using SplitExpense.ExceptionManagement.Exceptions;
using SplitExpense.Logger;
using SplitExpense.Models.DbModels;
using SplitExpense.SharedResource;

namespace SplitExpense.Data.Factory
{
    public class UserFactory(SplitExpenseDbContext context, ISplitExpenseLogger logger) : IUserFactory
    {
        private readonly SplitExpenseDbContext _context = context;
        private readonly ISplitExpenseLogger _logger = logger;

        public async Task<User> CreateAsync(User user)
        {
            try
            {
                // Check if email already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == user.Email && !u.IsDeleted);

                if (existingUser != null)
                {
                    _logger.LogError(LogMessage.RecordAlreadyExist.ToFormattedString(), 
                        ErrorCodes.EmailAlreadyExists.GetDescription(), 
                        "UserFactory-CreateAsync");
                    throw new BusinessRuleViolationException(ErrorCodes.EmailAlreadyExists);
                }

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating user with email {user.Email}");
                throw;
            }
        }

        public async Task<User?> GetUserByResetTokenAsync(string token)
        {
            try
            {
                return await _context.Users
                    .FirstOrDefaultAsync(u => u.ResetToken == token && 
                                            !u.IsDeleted && 
                                            u.ResetTokenExpiry.HasValue &&
                                            u.ResetTokenExpiry.Value > DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user by reset token");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(User user)
        {
            try
            {
                _context.Users.Update(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user {user.UserId}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null) return false;

                user.IsDeleted = true;
                _context.Users.Update(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user {id}");
                throw;
            }
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == id && !u.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user {id}");
                throw;
            }
        }
    }
}
