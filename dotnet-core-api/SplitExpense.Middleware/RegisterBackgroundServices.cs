using Microsoft.Extensions.DependencyInjection;
using SplitExpense.EmailManagement.BackgroundServices;

namespace SplitExpense.Middleware
{
    public static class RegisterBackgroundServices
    {
        public static IServiceCollection RegisterBackgroundService(this IServiceCollection services)
        {
            return services.AddHostedService<EmailBackgroundService>();
        }
    }
}
