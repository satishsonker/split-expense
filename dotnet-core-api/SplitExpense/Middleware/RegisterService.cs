using SplitExpense.Data.Factory;
using SplitExpense.Logger;
using SplitExpense.Logic;

namespace SplitExpense.Middleware
{
    public static class RegisterServices
    {
        public static IServiceCollection RegisterService(this IServiceCollection services)
        {
            return services.AddScoped<ISplitExpenseLogger,SplitExpenseLogger>()
                .AddScoped<IGroupFactory,GroupFactory>()
                .AddScoped<IGroupLogic,GroupsLogic>()
                .AddScoped<IErrorLogFactory,ErrorLogFactory>();
        }
    }
}
