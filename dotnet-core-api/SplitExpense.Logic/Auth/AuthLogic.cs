using AutoMapper;
using Microsoft.Extensions.Configuration;
using SplitExpense.Data.Factory;
using SplitExpense.ExceptionManagement.Exceptions;
using SplitExpense.Logger;
using SplitExpense.Models.DbModels;
using SplitExpense.Models.DTO;
using SplitExpense.SharedResource;
using System.Security.Cryptography;
using System.Text;

namespace SplitExpense.Logic
{
    public class AuthLogic(
        IAuthFactory authFactory,
        IUserFactory userFactory,
        IMapper mapper,
        IEmailLogic emailLogic,
        ISplitExpenseLogger logger,
        IConfiguration configuration) : IAuthLogic
    {
        private readonly IAuthFactory _authFactory = authFactory;
        private readonly IMapper _mapper = mapper;
        private readonly IEmailLogic _emailLogic = emailLogic;
        private readonly ISplitExpenseLogger _logger = logger;
        private readonly IConfiguration _configuration = configuration;
        private readonly IUserFactory _userFactory = userFactory;

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _authFactory.ValidateUserAsync(request.Email, request.Password) ?? throw new BusinessRuleViolationException(ErrorCodes.InvalidCredentials);
                await _authFactory.UpdateUserLastLoginAsync(user.UserId);

                return new LoginResponse
                {
                    TokenExpiration = DateTime.UtcNow.AddDays(1),
                    User = _mapper.Map<UserResponse>(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                throw;
            }
        }

        public async Task<bool> LogoutAsync(int userId)
        {
            try
            {
                // You might want to invalidate refresh tokens here
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during logout for user {userId}");
                throw;
            }
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            try
            {
                var user = await _authFactory.GetUserByEmailAsync(request.Email);
                if (user == null) return true; // Don't reveal if email exists

                // Generate reset token
                var resetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                var expiry = DateTime.UtcNow.AddHours(24);

                await _authFactory.SaveResetTokenAsync(user.UserId, resetToken, expiry);

                var frontendUrl = _configuration["AppSettings:FrontendUrl"] ?? "http://localhost:3000";
                var resetLink = $"{frontendUrl}/reset-password?token={resetToken}";

                // Send reset email
                var emailData = new Dictionary<string, string>
                {
                    { "userName", $"{user.FirstName} {user.LastName}" },
                    { "resetToken", resetToken },
                    { "resetLink", resetLink }
                };

                await _emailLogic.SendEmailOnPasswordResetAsync(user.Email, user.FirstName, DateTime.UtcNow, emailData);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing forgot password for email {request.Email}");
                throw;
            }
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            try
            {
                if (!await _authFactory.ValidateResetTokenAsync(request.Token))
                    throw new BusinessRuleViolationException(ErrorCodes.InvalidResetToken);

                var user =_userFactory.GetUserByResetTokenAsync(request.Token);

                return user == null
                    ? throw new BusinessRuleViolationException(ErrorCodes.InvalidResetToken)
                    : await _authFactory.UpdatePasswordAsync(user.Id, request.NewPassword);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                throw;
            }
        }

        public async Task<bool> ForgotUsernameAsync(ForgotUsernameRequest request)
        {
            try
            {
                var user = await _authFactory.GetUserByEmailAsync(request.Email);
                if (user == null) return true; // Don't reveal if email exists

                // Send username reminder email
                var emailData = new Dictionary<string, string>
                {
                    { "userName", $"{user.FirstName} {user.LastName}" },
                    { "username", user.Email }
                };

                await _emailLogic.SendEmailOnUsernameReminderAsync(user.Email, user.FirstName, DateTime.UtcNow, emailData);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing forgot username for email {request.Email}");
                throw;
            }
        }

        public async Task<LoginResponse> CreateUserAsync(CreateUserRequest request)
        {
            try
            {
                // Check if email already exists
                var existingUser = await _authFactory.GetUserByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    throw new BusinessRuleViolationException(ErrorCodes.EmailAlreadyExists);
                }

                // Create new user
                var user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Phone = request.PhoneNumber,
                    CreatedAt = DateTime.UtcNow
                };

                // Create password hash
                CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                // Save user
                user = await _userFactory.CreateAsync(user);

                // Return login response
                return new LoginResponse
                {
                    TokenExpiration = DateTime.UtcNow.AddMinutes(
                        _configuration.GetValue<int>("Jwt:TokenExpirationInMinutes")),
                    User = _mapper.Map<UserResponse>(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                throw;
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
} 