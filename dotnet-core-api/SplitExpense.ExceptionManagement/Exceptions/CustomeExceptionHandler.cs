using Microsoft.AspNetCore.Builder;

namespace SplitExpense.ExceptionManagement.Exceptions
{

    public static class CustomExceptionHandler
    {
        public static void UseCustomExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}