using Microsoft.Extensions.DependencyInjection;
using SplitExpense.Data.Factory;
using SplitExpense.Data.Services;
using SplitExpense.EmailManagement.Service;
using SplitExpense.Logger;
using SplitExpense.Logic;
using SplitExpense.Logic.Email;

namespace SplitExpense.Middleware
{
    public static class RegisterServices
    {
        public static IServiceCollection RegisterService(this IServiceCollection services)
        {
            services.AddHttpClient<EmailApiService>();
            return services.AddScoped<ISplitExpenseLogger, SplitExpenseLogger>()
                .AddSingleton<IUserContextService, UserContextService>()
                .AddScoped<IGroupFactory, GroupFactory>()
                .AddScoped<IGroupLogic, GroupsLogic>()
                #region Email Service
                 .AddScoped<IEmailLogic, EmailLogic>()
                 .AddScoped<IEmailQueueService, EmailQueueService>()
                 .AddScoped<IEmailTemplateService, EmailTemplateService>()
                 .AddScoped<EmailSmtpService>()
                 .AddScoped<EmailApiService>()
                 .AddScoped<EmailServiceFactory>()
                #endregion
                .AddScoped<IErrorLogFactory, ErrorLogFactory>();
        }
    }
}
