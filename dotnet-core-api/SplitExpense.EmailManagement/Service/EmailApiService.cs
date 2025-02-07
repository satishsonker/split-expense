using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SplitExpense.Logger;
using SplitExpense.Models.ConfigModels;
using System.Text;
using System.Text.Json;
namespace SplitExpense.EmailManagement.Service
{
    public class EmailApiService(HttpClient httpClient, IOptions<EmailSettings> options, ISplitExpenseLogger logger) : IEmailService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly EmailSettings _emailSettings = options.Value;
        private readonly ISplitExpenseLogger _logger = logger;

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            var apiKey = _emailSettings.DefaultApi;
            if (!_emailSettings.ApiProviders.TryGetValue(apiKey, out var apiConfig))
            {
                _logger.LogError($"Email API provider '{apiKey}' not found in configuration.");
                return false;
            }

            var requestData = new
            {
                api_key = apiConfig.ApiKey,
                inbox_id = apiConfig.InboxId,
                subject,
                body,
                to,
                from = apiConfig.From
            };

            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

            try
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiConfig.ApiKey}");
                var response = await _httpClient.PostAsync($"{apiConfig.BaseUrl}/send", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInfo($"Email sent successfully to {to} via API ({apiKey}).");
                    return true;
                }
                else
                {
                    _logger.LogError($"Failed to send email to {to} via API ({apiKey}). Response: {responseBody}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email to {to}: {ex.Message}");
                return false;
            }
        }
    }

}
