using SplitExpense.Models.DbModels;
using SplitExpense.Models.DTO;

namespace SplitExpense.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        string GenerateToken(UserResponse user);
        string GenerateRefreshToken();
        bool ValidateToken(string token);
        int? GetUserIdFromToken(string token);
        bool ValidateRefreshToken(string refreshToken);
    }
} 