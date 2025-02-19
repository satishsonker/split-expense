using Microsoft.AspNetCore.Mvc;
using SplitExpense.Logic;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DTO.Response;
using SplitExpense.SharedResource;

namespace SplitExpense.Controllers
{
    [ApiController]
    public class EmailController(IEmailLogic emailLogic) : ControllerBase
    {
        private readonly IEmailLogic _emailLogic = emailLogic;

        [ProducesResponseType(typeof(EmailQueueResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet(ApiPaths.EmailGetQueue)]
        public async Task<EmailQueueResponse> CreateAsync([FromQuery] PagingRequest request)
        {
            return await _emailLogic.GetEmailQueueAsync(request);
        }
    }
}
