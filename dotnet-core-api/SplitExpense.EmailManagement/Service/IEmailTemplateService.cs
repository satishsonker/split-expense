using SplitExpense.Models;
using SplitExpense.SharedResource;

namespace SplitExpense.EmailManagement.Service
{
    public interface IEmailTemplateService
    {
        Task<int> AddTemplateAsync(EmailTemplate template);
        Task<bool> UpdateTemplateAsync(EmailTemplate template);
        Task<bool> DeleteTemplateAsync(int templateId);
        Task<EmailTemplate?> GetTemplateByIdAsync(int id);
        Task<EmailTemplate?> GetTemplateByCodeAsync(EmailTemplateCode code);
        Task<List<EmailTemplate>> GetAllTemplatesAsync();
        Task<List<EmailTemplate>> SearchTemplatesAsync(string subject);
    }
}
