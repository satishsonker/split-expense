using Microsoft.Extensions.DependencyInjection;
using SplitExpense.Data.Factory;
using SplitExpense.EmailManagement.Service;
using SplitExpense.Logger;
using SplitExpense.Logic;

namespace SplitExpense.Middleware
{
    public static class RegisterServices
    {
        public static IServiceCollection RegisterService(this IServiceCollection services)
        {
            return services.AddScoped<ISplitExpenseLogger,SplitExpenseLogger>()
                .AddSingleton<IUserContextService, UserContextService>()
                .AddScoped<IGroupFactory,GroupFactory>()
                .AddScoped<IGroupLogic,GroupsLogic>()
                .AddScoped<IEmailQueueService, EmailQueueService>()
                .AddScoped<IEmailTemplateService, EmailTemplateService>()
                .AddScoped<IErrorLogFactory,ErrorLogFactory>();
        }
    }
}
