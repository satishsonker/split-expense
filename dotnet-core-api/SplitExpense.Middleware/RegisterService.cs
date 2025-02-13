using Microsoft.Extensions.DependencyInjection;
using SplitExpense.Data.Factory;
using SplitExpense.Data.Services;
using SplitExpense.EmailManagement.Service;
using SplitExpense.FileManagement.Service;
using SplitExpense.FileManagement.Storage;
using SplitExpense.Logger;
using SplitExpense.Logic;
using SplitExpense.Logic.Email;
using SplitExpense.Logic.Masters;

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
                .AddScoped<ISplitTypeFactory,SplitTypeFactory>()
                .AddScoped<ISplitTypeLogic,SplitTypeLogic>()
                .AddScoped<IGroupTypeFactory, GroupTypeFactory>()
                .AddScoped<IGroupTypeLogic, GroupTypeLogic>()
                .AddScoped<IFileUploadService, FileUploadService>()
            #region Email Service
                 .AddScoped<IEmailLogic, EmailLogic>()
                 .AddScoped<IEmailQueueService, EmailQueueService>()
                 .AddScoped<IEmailTemplateService, EmailTemplateService>()
                 .AddScoped<IContactFactory,ContactFactory>()
                 .AddScoped<IContactLogic,ContactLogic>()
                 .AddScoped<EmailSmtpService>()
                 .AddScoped<EmailApiService>()
                 .AddScoped<EmailServiceFactory>()
                #endregion
                .AddScoped<IErrorLogFactory, ErrorLogFactory>();
        }
    }
}
