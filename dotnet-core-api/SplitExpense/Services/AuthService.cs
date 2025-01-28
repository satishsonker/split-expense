using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SplitExpense.Logger;

namespace SplitExpense.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ISplitExpenseLogger _logger;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<IdentityUser> userManager, IConfiguration configuration, ISplitExpenseLogger logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IdentityResult> RegisterUserAsync(string username, string email, string password)
        {
            var user = new IdentityUser { UserName = username, Email = email };
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<string> LoginUserAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                return null;

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<IdentityResult> DeleteUserAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            return await _userManager.DeleteAsync(user);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return null;

            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task<IdentityResult> ChangePasswordAsync(string username, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }
    }

}
