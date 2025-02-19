using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SplitExpense.Logger;
using SplitExpense.Models.DbModels;
using SplitExpense.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SplitExpense.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ISplitExpenseLogger _logger;
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtService(IConfiguration configuration, ISplitExpenseLogger logger)
        {
            _configuration = configuration;
            _logger = logger;
            _key = _configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key not configured");
            _issuer = _configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer not configured");
            _audience = _configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience not configured");
        }

        public string GenerateToken(User user)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.GivenName, user.FirstName),
                    new Claim(ClaimTypes.Surname, user.LastName),
                    new Claim("userId", user.UserId.ToString())
                };

                var token = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token");
                throw;
            }
        }

        public string GenerateRefreshToken()
        {
            try
            {
                var randomNumber = new byte[64];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating refresh token");
                throw;
            }
        }

        public bool ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return false;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_key);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return false;
            }
        }

        public int? GetUserIdFromToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_key);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "userId").Value);

                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user ID from token");
                return null;
            }
        }

        public bool ValidateRefreshToken(string refreshToken)
        {
            // In a production environment, you would validate against stored refresh tokens
            // For now, we'll just check if it's a valid Base64 string
            try
            {
                Convert.FromBase64String(refreshToken);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
} 