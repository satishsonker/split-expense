using SplitExpense.EmailManagement.Models;

namespace SplitExpense.EmailManagement.Service
{
    public class EmailTemplateService : IEmailTemplateService
    {
        public Task AddTemplate(EmailTemplate template)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTemplate(int templateId)
        {
            throw new NotImplementedException();
        }

        public Task<List<EmailTemplate>> GetAllTemplates()
        {
            throw new NotImplementedException();
        }

        public Task<EmailTemplate> GetTemplateById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<EmailTemplate>> SearchTemplates(string subject)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTemplate(EmailTemplate template)
        {
            throw new NotImplementedException();
        }
    }
}
