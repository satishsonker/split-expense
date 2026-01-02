using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DbModels;
using SplitExpense.SharedResource;

namespace SplitExpense.Data.Factory.Email
{
    public interface IEmailTemplateFactory
    {
        Task<int> AddTemplateAsync(EmailTemplate template);
        Task<bool> UpdateTemplateAsync(EmailTemplate template);
        Task<bool> DeleteTemplateAsync(int templateId);
        Task<EmailTemplate?> GetTemplateByIdAsync(int id);
        Task<EmailTemplate?> GetTemplateByCodeAsync(EmailTemplateCode code);
        Task<PagingResponse<EmailTemplate>> GetAllTemplatesAsync(PagingRequest pagingRequest);
        Task<PagingResponse<EmailTemplate>> SearchTemplatesAsync(SearchRequest subject);
    }
}
