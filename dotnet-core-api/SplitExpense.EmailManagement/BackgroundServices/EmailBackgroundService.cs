using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz.Impl.Triggers;
using SplitExpense.Data;
using SplitExpense.EmailManagement.Service;

namespace SplitExpense.EmailManagement.BackgroundServices
{
    public class EmailBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _cronExpression;
        private readonly bool _isEnabled;

        public EmailBackgroundService(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _cronExpression = configuration.GetSection("EmailSettings:CronExpression").Value ?? string.Empty;
            _ = Boolean.TryParse(configuration.GetSection("EmailSettings:Enabled").Value, out _isEnabled);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_isEnabled) return;

            var cronTrigger = new CronTriggerImpl
            {
                CronExpressionString = _cronExpression
            };

            var nextRun = cronTrigger.GetFireTimeAfter(DateTimeOffset.UtcNow);

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<SplitExpenseDbContext>();
                var _emailQueueService = scope.ServiceProvider.GetRequiredService<IEmailQueueService>();
                if (DateTimeOffset.UtcNow >= nextRun)
                {
                    await _emailQueueService.ProcessQueue();
                    nextRun = cronTrigger.GetFireTimeAfter(DateTimeOffset.UtcNow);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
