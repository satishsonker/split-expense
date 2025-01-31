using SplitExpense.EmailManagement.Models;

namespace SplitExpense.EmailManagement.Service
{
    public interface IEmailTemplateService
    {
        Task AddTemplate(EmailTemplate template);
        Task UpdateTemplate(EmailTemplate template);
        Task DeleteTemplate(int templateId);
        Task<EmailTemplate> GetTemplateById(int id);
        Task<List<EmailTemplate>> GetAllTemplates();
        Task<List<EmailTemplate>> SearchTemplates(string subject);
    }
}
