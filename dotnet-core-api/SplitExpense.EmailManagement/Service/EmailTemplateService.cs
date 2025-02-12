using Microsoft.EntityFrameworkCore;
using SplitExpense.Data;
using SplitExpense.ExceptionManagement.Exceptions;
using SplitExpense.Models.DbModels;
using SplitExpense.SharedResource;

namespace SplitExpense.EmailManagement.Service
{
    public class EmailTemplateService(SplitExpenseDbContext context) : IEmailTemplateService
    {
        private readonly SplitExpenseDbContext _context = context;
        public async Task<int> AddTemplateAsync(EmailTemplate template)
        {
            ArgumentNullException.ThrowIfNull(nameof(template));

            if (!Enum.TryParse<EmailTemplateCode>(template.TemplateCode, true, out EmailTemplateCode emailTemplateCode))
            {
                throw new BusinessRuleViolationException(ErrorCodes.InvalidFieldFormat.ToString(), UserMessage.InvalidEmailTemplateCode.ToFormattedString());
            }

            var oldData = await GetTemplateByCodeAsync(emailTemplateCode);
            if (oldData != null)
            {
                throw new BusinessRuleViolationException(ErrorCodes.RecordAlreadyExist);
            }
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

        public async Task<List<EmailTemplate>> GetAllTemplatesAsync()
        {
            return await _context.EmailTemplates
            .Where(x => !x.IsDeleted)
            .ToListAsync();
        }

        public async Task<EmailTemplate?> GetTemplateByCodeAsync(EmailTemplateCode code)
        {
            ArgumentNullException.ThrowIfNull(code);
            return await _context.EmailTemplates
                .Where(x => !x.IsDeleted && x.TemplateCode == code.ToString())
                .FirstOrDefaultAsync()??throw new BusinessRuleViolationException(ErrorCodes.RecordNotFound);
        }

        public async Task<EmailTemplate?> GetTemplateByIdAsync(int id)
        {
            return await _context.EmailTemplates
                .Where(x => !x.IsDeleted && x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<EmailTemplate>> SearchTemplatesAsync(string subject)
        {
            return await _context.EmailTemplates
                .Where(x => !x.IsDeleted && (string.IsNullOrEmpty(subject) || x.Subject.Contains(subject) || x.Body.Contains(subject)))
                .ToListAsync();
        }

        public async Task<bool> UpdateTemplateAsync(EmailTemplate template)
        {
            ArgumentNullException.ThrowIfNull(nameof(template));

            var oldData = await GetTemplateByIdAsync(template.Id) ?? throw new BusinessRuleViolationException(ErrorCodes.RecordNotFound);
            oldData.Subject = template.Subject;
            oldData.Body = template.Body;
            oldData.IsHtml = template.IsHtml;
            _context.Update(oldData);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
