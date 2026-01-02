using Microsoft.EntityFrameworkCore;
using SplitExpense.ExceptionManagement.Exceptions;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DbModels;
using SplitExpense.SharedResource;

namespace SplitExpense.Data.Factory.Email
{
    public class EmailTemplateFactory(SplitExpenseDbContext context) : IEmailTemplateFactory
    {
        private readonly SplitExpenseDbContext _context = context;
        public async Task<int> AddTemplateAsync(EmailTemplate template)
        {
            await _context.EmailTemplates.AddAsync(template);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteTemplateAsync(int templateId)
        {
            var oldData = await GetTemplateByIdAsync(templateId) ?? throw new BusinessRuleViolationException(ErrorCodes.RecordNotFound);
            oldData.IsDeleted = true;
            _context.Update(oldData);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PagingResponse<EmailTemplate>> GetAllTemplatesAsync(PagingRequest pagingRequest)
        {
            var query = _context.EmailTemplates
            .Where(x => !x.IsDeleted).AsQueryable();

            var totalRecords = await query.CountAsync();

            query = query.Skip((pagingRequest.PageNo - 1) * pagingRequest.PageSize).Take(pagingRequest.PageSize);
            
            var templates = await query.ToListAsync();
            return new PagingResponse<EmailTemplate>
            {
                Data = templates,
                RecordCounts = totalRecords,
                PageSize = pagingRequest.PageSize,
                PageNo = pagingRequest.PageNo
            };
        }

        public async Task<EmailTemplate?> GetTemplateByCodeAsync(EmailTemplateCode code)
        {
            return await _context.EmailTemplates
               .Where(x => !x.IsDeleted && x.TemplateCode == code.ToString())
               .FirstOrDefaultAsync() ?? throw new BusinessRuleViolationException(ErrorCodes.RecordNotFound);
        }

        public async Task<EmailTemplate?> GetTemplateByIdAsync(int id)
        {
            return await _context.EmailTemplates
                .Where(x => !x.IsDeleted && x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<PagingResponse<EmailTemplate>> SearchTemplatesAsync(SearchRequest request)
        {
            var query = _context.EmailTemplates
           .Where(x => !x.IsDeleted).AsQueryable();

            var totalRecords = await query.CountAsync();

            query = query
                .Where(x=>string.IsNullOrEmpty(request.SearchTerm) || x.Subject.Contains(request.SearchTerm) || x.Body.Contains(request.SearchTerm))
                .Skip((request.PageNo - 1) * request.PageSize).Take(request.PageSize);

            var templates = await query.ToListAsync();
            return new PagingResponse<EmailTemplate>
            {
                Data = templates,
                RecordCounts = totalRecords,
                PageSize = request.PageSize,
                PageNo = request.PageNo
            };
        }

        public async Task<bool> UpdateTemplateAsync(EmailTemplate template)
        {
            var oldData = await GetTemplateByIdAsync(template.Id) ?? throw new BusinessRuleViolationException(ErrorCodes.RecordNotFound);
            oldData.Subject = template.Subject;
            oldData.Body = template.Body;
            oldData.IsHtml = template.IsHtml;
            _context.Update(oldData);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
