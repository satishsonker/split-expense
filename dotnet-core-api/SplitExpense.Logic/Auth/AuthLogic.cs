using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SplitExpense.Data.Factory;
using SplitExpense.ExceptionManagement.Exceptions;
using SplitExpense.FileManagement.Service;
using SplitExpense.Logger;
using SplitExpense.Models.DbModels;
using SplitExpense.Models.DTO;
using SplitExpense.Models.DTO.Response;
using SplitExpense.Services;
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
        IConfiguration configuration,
        IFileUploadService fileUploadService,
        IRequestInfoService requestInfoService
        ) : IAuthLogic
    {
        private readonly IAuthFactory _authFactory = authFactory;
        private readonly IMapper _mapper = mapper;
        private readonly IEmailLogic _emailLogic = emailLogic;
        private readonly ISplitExpenseLogger _logger = logger;
        private readonly IConfiguration _configuration = configuration;
        private readonly IUserFactory _userFactory = userFactory;
        private readonly IFileUploadService _fileUploadService = fileUploadService;
        private readonly IRequestInfoService _requestInfoService = requestInfoService;

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var (success, msg, user) = await _authFactory.ValidateUserAsync(request.Email, request.Password);

                var loginResponse = new LoginResponse
                {
                    IsSuccess = success,
                    Message = msg,
                    TokenExpiration = DateTime.UtcNow.AddDays(1),
                };
                if (user != null)
                {
                    await _authFactory.UpdateUserLastLoginAsync(user.UserId);
                    loginResponse.User = _mapper.Map<UserResponse>(user);
                }
                return loginResponse;
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
                var user = await _authFactory.GetUserByEmailOrPhoneAsync(request.Email,string.Empty);
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
                    { "Name", $"{user.FirstName} {user.LastName}" },
                    { "UserEmail", $"{user.Email}" },
                    { "RequestTime", $"{DateTime.UtcNow.ToString("G")}" },
                    { "ResetToken", resetToken },
                    { "ResetLink", resetLink },
                    { "ExpirationTime", expiry.ToString("G") }
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

        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            ResetPasswordResponse response = new();
            try
            {
                if (!await _authFactory.ValidateResetTokenAsync(request.Token))
                {
                    response.Success = false;
                    response.Message = "This link either expired or invalid.";
                    throw new BusinessRuleViolationException(ErrorCodes.InvalidResetToken);
                }

                var user = await _userFactory.GetUserByResetTokenAsync(request.Token);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                }
                else
                {
                   var result= await _authFactory.UpdatePasswordAsync(user.UserId, request.NewPassword);
                    if (result)
                    {
                        var emailData = new Dictionary<string, string>
                        {
                            { "Name", $"{user.FirstName} {user.LastName}" },
                            { "ResetOn", $"{DateTime.UtcNow:G}" },
                            { "IPAddress", $"{DateTime.UtcNow:G}" },
                            { "DeviceInfo", $"{_requestInfoService.GetDeviceInfo}" },
                            { "LoginUrl", $"{_requestInfoService.GetClientIpAddress}" }
                        };

                        await _emailLogic.SendEmailOnPasswordResetSuccessAsync(user.Email, user.FirstName, DateTime.UtcNow, emailData);
                    }
                    response.Success = true;
                    response.Message = "Password has been successfully reset.";
                }
                

                return response;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                return response;
            }
        }

        public async Task<bool> ForgotUsernameAsync(ForgotUsernameRequest request)
        {
            try
            {
                var user = await _authFactory.GetUserByEmailOrPhoneAsync(request.Email,string.Empty);
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
                var existingUser = await _authFactory.GetUserByEmailOrPhoneAsync(request.Email,request.PhoneNumber);
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

        public async Task<UserResponse> UpdateUserAsync(UpdateUserRequest request)
        {
            try
            {
                // Check email uniqueness if changed
                if (!string.IsNullOrEmpty(request.Email))
                {
                    var existingUser = await _authFactory.GetUserByEmailOrPhoneAsync(request.Email, string.Empty);
                    if (existingUser != null && existingUser.UserId != request.UserId)
                    {
                        throw new BusinessRuleViolationException(ErrorCodes.EmailAlreadyExists);
                    }
                }

                var user = new User
                {
                    UserId = request.UserId,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Phone = request.PhoneNumber,
                    LanguageCode = request.LanguageCode,
                    CurrencyCode = request.CurrencyCode,
                    CountryCode = request.CountryCode,
                    ISDCode = request.ISDCode
                };

                var updatedUser = await _authFactory.UpdateUserAsync(user);
                return _mapper.Map<UserResponse>(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user {request.UserId}");
                throw;
            }
        }

        public async Task<UserResponse> UpdateProfilePictureAsync(int userId, IFormFile file)
        {
            try
            {
                var user = await _userFactory.GetByIdAsync(userId);
                if (user == null)
                    throw new BusinessRuleViolationException(ErrorCodes.UserNotFound);

                // Delete old profile picture if exists
                if (!string.IsNullOrEmpty(user.ProfilePicture))
                {
                    await _fileUploadService.DeleteFileAsync(user.ProfilePicture,user.ThumbProfilePicture);
                }

                // Upload new profile picture
                var uploadResult = await _fileUploadService.UploadFileAsync(file);
                var updatedUser = await _authFactory.UpdateProfilePictureAsync(userId, uploadResult);

                return _mapper.Map<UserResponse>(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating profile picture for user {userId}");
                throw;
            }
        }

        public async Task<bool> DeleteProfilePictureAsync(int userId)
        {
            try
            {
                var user = await _userFactory.GetByIdAsync(userId);
                if (user == null)
                    throw new BusinessRuleViolationException(ErrorCodes.UserNotFound);

                if (!string.IsNullOrEmpty(user.ProfilePicture))
                {
                    await _fileUploadService.DeleteFileAsync(user.ProfilePicture);
                }

                return await _authFactory.DeleteProfilePictureAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting profile picture for user {userId}");
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _userFactory.GetByIdAsync(userId);
                if (user == null)
                    throw new BusinessRuleViolationException(ErrorCodes.UserNotFound);

                if (!string.IsNullOrEmpty(user.ProfilePicture))
                {
                    await _fileUploadService.DeleteFileAsync(user.ProfilePicture);
                }

                return await _userFactory.DeleteAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user {userId}");
                throw;
            }
        }
    }
} 