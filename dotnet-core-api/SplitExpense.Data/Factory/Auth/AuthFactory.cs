using Microsoft.EntityFrameworkCore;
using SplitExpense.Logger;
using SplitExpense.Models.DbModels;
using System.Security.Cryptography;
using System.Text;

namespace SplitExpense.Data.Factory
{
    public class AuthFactory(SplitExpenseDbContext context, ISplitExpenseLogger logger) : IAuthFactory
    {
        private readonly SplitExpenseDbContext _context = context;
        private readonly ISplitExpenseLogger _logger = logger;

        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);

                if (user == null || !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                    return null;

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating user credentials");
                throw;
            }
        }

        public async Task<bool> UpdatePasswordAsync(int userId, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.ResetToken = null;
                user.ResetTokenExpiry = null;

                _context.Users.Update(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating password for user {userId}");
                throw;
            }
        }

        public async Task<bool> ValidateResetTokenAsync(string token)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.ResetToken == token && !u.IsDeleted);

                return user != null && 
                       user.ResetTokenExpiry.HasValue && 
                       user.ResetTokenExpiry.Value > DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating reset token");
                throw;
            }
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email");
                throw;
            }
        }

        public async Task<bool> SaveResetTokenAsync(int userId, string token, DateTime expiry)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                user.ResetToken = token;
                user.ResetTokenExpiry = expiry;

                _context.Users.Update(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving reset token for user {userId}");
                throw;
            }
        }

        public async Task<bool> UpdateUserLastLoginAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                user.LastLoginAt = DateTime.UtcNow;
                _context.Users.Update(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating last login for user {userId}");
                throw;
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
} 