using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitExpense.Logic;
using SplitExpense.Models.DTO;
using SplitExpense.Services;
using SplitExpense.SharedResource;

namespace SplitExpense.Controllers
{
    [ApiController]
    public class AuthController(IAuthLogic authLogic,IJwtService jwtService) : ControllerBase
    {
        private readonly IAuthLogic _authLogic = authLogic;
        private readonly IJwtService _jwtService = jwtService;

        [HttpPost(ApiPaths.Login)]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> LoginAsync([FromBody] LoginRequest request)
        {
            var response= await _authLogic.LoginAsync(request);
            if(response!=null)
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
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
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
        public async Task<ActionResult<bool>> ResetPasswordAsync([FromBody] ResetPasswordRequest request)
        {
            return await _authLogic.ResetPasswordAsync(request);
        }

        [HttpPost(ApiPaths.ForgotUsername)]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> ForgotUsernameAsync([FromBody] ForgotUsernameRequest request)
        {
            return await _authLogic.ForgotUsernameAsync(request);
        }
    }
} 