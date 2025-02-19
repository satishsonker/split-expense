using SplitExpense.Models.DbModels;

namespace SplitExpense.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        bool ValidateToken(string token);
        int? GetUserIdFromToken(string token);
        bool ValidateRefreshToken(string refreshToken);
    }
} 