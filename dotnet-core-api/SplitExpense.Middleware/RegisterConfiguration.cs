using Microsoft.Extensions.Configuration;

namespace SplitExpense.Middleware
{
    public static class RegisterConfigurations
    {
        public static IConfigurationBuilder RegisterConfiguration(this IConfigurationBuilder builder, string environment)
        {
            return builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }
    }
}
