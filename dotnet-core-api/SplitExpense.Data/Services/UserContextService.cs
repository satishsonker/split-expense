using Microsoft.AspNetCore.Http;

namespace SplitExpense.Data.Services
{
    public class UserContextService(IHttpContextAccessor httpContextAccessor) : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

        public int GetUserId()
        {
            try
            {
                if (_httpContextAccessor.HttpContext?.Request.Headers.ContainsKey("userId") == true)
                {
                    string userIdValue = _httpContextAccessor.HttpContext.Request.Headers["userId"].ToString();

                    if (string.IsNullOrEmpty(userIdValue))
                        throw new UnauthorizedAccessException("UserId not found in the request headers.");

                    if (!int.TryParse(userIdValue, out int userId))
                        throw new UnauthorizedAccessException("Invalid UserId format. Must be an integer.");

                    return userId;
                }
                throw new UnauthorizedAccessException("UserId header is missing.");
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException($"Error fetching UserId: {ex.Message}");
            }
        }

    }
}
