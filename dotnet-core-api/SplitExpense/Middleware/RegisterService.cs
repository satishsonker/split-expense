using SplitExpense.Logger;

namespace SplitExpense.Middleware
{
    public static class RegisterServices
    {
        public static IServiceCollection RegisterService(this IServiceCollection services)
        {
            return services.AddScoped<ISplitExpenseLogger,SplitExpenseLogger>();
        }
    }
}
