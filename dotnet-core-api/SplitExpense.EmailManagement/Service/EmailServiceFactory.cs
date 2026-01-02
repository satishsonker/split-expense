using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SplitExpense.Models.ConfigModels;

namespace SplitExpense.EmailManagement.Service
{
    public class EmailServiceFactory(IServiceProvider serviceProvider, IOptions<EmailSettings> options)
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly EmailSettings _emailSettings = options.Value;

        public IEmailService GetEmailService()
        {
            return _emailSettings.Provider switch
            {
                "SMTP" => _serviceProvider.GetRequiredService<EmailSmtpService>(),
                "API" => _serviceProvider.GetRequiredService<EmailApiService>(),
                _ => throw new InvalidOperationException("Invalid email provider configured.")
            };
        }
    }

}
