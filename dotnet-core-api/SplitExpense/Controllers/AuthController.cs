using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitExpense.Data.Services;
using SplitExpense.Logic;
using SplitExpense.Models.DTO;
using SplitExpense.Models.DTO.Response;
using SplitExpense.Services;
using SplitExpense.SharedResource;

namespace SplitExpense.Controllers
{
    [AllowAnonymous]
    [ApiController]
    public class AuthController(IAuthLogic authLogic, IJwtService jwtService,IUserContextService userContext) : ControllerBase
    {
        private readonly IAuthLogic _authLogic = authLogic;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IUserContextService _userContext = userContext;        

        [HttpPost(ApiPaths.Login)]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> LoginAsync([FromBody] LoginRequest request)
        {
            var response= await _authLogic.LoginAsync(request);
            if(response.User!=null)
            {
               response.Token= _jwtService.GenerateToken(response.User);
               response.RefreshToken = _jwtService.GenerateRefreshToken();
            }
            return response;
        }

        [HttpPost(ApiPaths.Register)]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> CreateUserAsync([FromBody] CreateUserRequest request)
        {
            var response = await _authLogic.CreateUserAsync(request);
            if (response != null)
            {
                response.Token = _jwtService.GenerateToken(response.User);
                response.RefreshToken = _jwtService.GenerateRefreshToken();
            }
            return response;
        }

        [HttpPost(ApiPaths.Logout)]
        [Authorize]
        public async Task<ActionResult<bool>> LogoutAsync()
        {
            var userId = _userContext.GetUserId();
            return await _authLogic.LogoutAsync(userId);
        }

        [HttpPost(ApiPaths.ForgotPassword)]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> ForgotPasswordAsync([FromBody] ForgotPasswordRequest request)
        {
            return await _authLogic.ForgotPasswordAsync(request);
        }

        [HttpPost(ApiPaths.ResetPassword)]
        [AllowAnonymous]
        public async Task<ActionResult<ResetPasswordResponse>> ResetPasswordAsync([FromBody] ResetPasswordRequest request)
        {
            return await _authLogic.ResetPasswordAsync(request);
        }

        [HttpPost(ApiPaths.ForgotUsername)]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> ForgotUsernameAsync([FromBody] ForgotUsernameRequest request)
        {
            return await _authLogic.ForgotUsernameAsync(request);
        }

        [HttpPut(ApiPaths.UpdateUserProfile)]
        [Authorize]
        public async Task<ActionResult<UserResponse>> UpdateUserAsync([FromBody] UpdateUserRequest request)
        {
            var userId = _userContext.GetUserId();
            request.UserId = userId;
            return await _authLogic.UpdateUserAsync(request);
        }

        [HttpPost("profile-picture")]
        [Authorize]
        public async Task<ActionResult<UserResponse>> UpdateProfilePictureAsync(IFormFile file)
        {
            var userId = _userContext.GetUserId();
            return await _authLogic.UpdateProfilePictureAsync(userId, file);
        }

        [HttpDelete("profile-picture")]
        [Authorize]
        public async Task<ActionResult<bool>> DeleteProfilePictureAsync()
        {
            var userId = _userContext.GetUserId();
            return await _authLogic.DeleteProfilePictureAsync(userId);
        }

        [HttpDelete("delete")]
        [Authorize]
        public async Task<ActionResult<bool>> DeleteUserAsync()
        {
            var userId = _userContext.GetUserId();
            return await _authLogic.DeleteUserAsync(userId);
        }
    }
} 