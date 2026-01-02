using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitExpense.EmailManagement.Service;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DTO;
using SplitExpense.SharedResource;

namespace SplitExpense.Controllers
{
    [Authorize]
    [ApiController]
    public class EmailTemplateController(IEmailTemplateService emailTemplateService) : ControllerBase
    {
        private readonly IEmailTemplateService _emailTemplateService = emailTemplateService;

        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [HttpPost(ApiPaths.EmailTemplateCreate)]
        public Task<int> AddTemplateAsync([FromBody]EmailTemplateRequest template)
        {
            return _emailTemplateService.AddTemplateAsync(template);
        }


        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [HttpDelete(ApiPaths.EmailTemplateDelete)]
        public Task<bool> DeleteTemplateAsync([FromRoute] int id)
        {
            return _emailTemplateService.DeleteTemplateAsync(id);
        }

        [ProducesResponseType(typeof(PagingResponse<EmailTemplateResponse>), StatusCodes.Status200OK)]
        [HttpGet(ApiPaths.EmailTemplateGetAll)]
        public Task<PagingResponse<EmailTemplateResponse>> GetAllTemplatesAsync([FromQuery] PagingRequest request)
        {
            return _emailTemplateService.GetAllTemplatesAsync(request);
        }

        [ProducesResponseType(typeof(PagingResponse<EmailTemplateResponse>), StatusCodes.Status200OK)]
        [HttpGet(ApiPaths.EmailTemplateSearch)]
        public Task<PagingResponse<EmailTemplateResponse>> SearchTemplatesAsync([FromQuery] SearchRequest request)
        {
            return _emailTemplateService.SearchTemplatesAsync(request);
        }

        [ProducesResponseType(typeof(PagingResponse<EmailTemplateResponse>), StatusCodes.Status200OK)]
        [HttpPost(ApiPaths.EmailTemplateUpdate)]
        public Task<bool> UpdateTemplateAsync([FromBody] EmailTemplateRequest template)
        {
            return _emailTemplateService.UpdateTemplateAsync(template);
        }
    }
}
