using AutoMapper;
using SplitExpense.Data.Factory.Email;
using SplitExpense.ExceptionManagement.Exceptions;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DbModels;
using SplitExpense.Models.DTO;
using SplitExpense.SharedResource;

namespace SplitExpense.EmailManagement.Service
{
    public class EmailTemplateService(IEmailTemplateFactory emailTemplateFactory, IMapper mapper) : IEmailTemplateService
    {       
        private readonly IMapper _mapper=mapper;
        private readonly IEmailTemplateFactory _emailTemplateFactory = emailTemplateFactory;
        public async Task<int> AddTemplateAsync(EmailTemplateRequest template)
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
            var templateEntity = _mapper.Map<EmailTemplate>(template);
           return await _emailTemplateFactory.AddTemplateAsync(templateEntity);
        }

        public async Task<bool> DeleteTemplateAsync(int templateId)
        {
            return await _emailTemplateFactory.DeleteTemplateAsync(templateId);
        }

        public async Task<PagingResponse<EmailTemplateResponse>> GetAllTemplatesAsync(PagingRequest pagingRequest)
        {
            return _mapper.Map<PagingResponse<EmailTemplateResponse>>(await _emailTemplateFactory.GetAllTemplatesAsync(pagingRequest));
        }

        public async Task<EmailTemplateResponse?> GetTemplateByCodeAsync(EmailTemplateCode code)
        {
            ArgumentNullException.ThrowIfNull(code);
           return _mapper.Map<EmailTemplateResponse?>(await _emailTemplateFactory.GetTemplateByCodeAsync(code));
        }

        public async Task<EmailTemplate?> GetTemplateByIdAsync(int id)
        {
            return await _emailTemplateFactory.GetTemplateByIdAsync(id);
        }

        public async Task<PagingResponse<EmailTemplateResponse>> SearchTemplatesAsync(SearchRequest searchRequest)
        {
            return _mapper.Map<PagingResponse<EmailTemplateResponse>>(await _emailTemplateFactory.SearchTemplatesAsync(searchRequest));
        }

        public async Task<bool> UpdateTemplateAsync(EmailTemplateRequest template)
        {
            ArgumentNullException.ThrowIfNull(nameof(template));
            return await _emailTemplateFactory.UpdateTemplateAsync(_mapper.Map<EmailTemplate>(template));

        }
    }
}
