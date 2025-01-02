using NLog;

namespace SplitExpense.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An unhandled exception has occurred.");
                throw;  // Re-throw the exception to preserve the normal behavior
            }
        }
    }
}