using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DbModels;
using SplitExpense.Models.DTO;
using SplitExpense.SharedResource;

namespace SplitExpense.EmailManagement.Service
{
    public interface IEmailTemplateService
    {
        Task<int> AddTemplateAsync(EmailTemplateRequest template);
        Task<bool> UpdateTemplateAsync(EmailTemplateRequest template);
        Task<bool> DeleteTemplateAsync(int templateId);
        Task<EmailTemplate?> GetTemplateByIdAsync(int id);
        Task<EmailTemplateResponse?> GetTemplateByCodeAsync(EmailTemplateCode code);
        Task<PagingResponse<EmailTemplateResponse>> GetAllTemplatesAsync(PagingRequest pagingRequest);
        Task<PagingResponse<EmailTemplateResponse>> SearchTemplatesAsync(SearchRequest searchRequest);
    }
}
