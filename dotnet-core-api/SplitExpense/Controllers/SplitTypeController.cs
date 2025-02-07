using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SplitExpense.Logic;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DTO;
using SplitExpense.SharedResource;

namespace SplitExpense.Controllers
{
    [ApiController]
    public class SplitTypeController(ISplitTypeLogic splitTypeLogic) : ControllerBase
    {
        private readonly ISplitTypeLogic _splitTypeLogic=splitTypeLogic;

        [ProducesResponseType(typeof(SplitTypeResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(SplitTypeResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost(ApiPaths.SplitTypeCreate)]
        public async Task<SplitTypeResponse> CreateAsync([FromBody] SplitTypeRequest request)
        {
            return await _splitTypeLogic.CreateAsync(request);
        }

        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpDelete(ApiPaths.SplitTypeDelete)]
        public async Task<bool> DeleteAsync([FromRoute] int id)
        {
           return await _splitTypeLogic.DeleteAsync(id);
        }

        [ProducesResponseType(typeof(PagingResponse<SplitTypeResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet(ApiPaths.SplitTypeGetAll)]
        public async Task<PagingResponse<SplitTypeResponse>> GetAllAsync([FromQuery] PagingRequest request)
        {
            return await _splitTypeLogic.GetAllAsync(request);
        }

        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet(ApiPaths.SplitTypeGetById)]
        public async Task<SplitTypeResponse> GetAsync([FromRoute] int id)
        {
           return await _splitTypeLogic.GetAsync(id);
        }

        [ProducesResponseType(typeof(PagingResponse<SplitTypeResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet(ApiPaths.SplitTypeSearch)]
        public async Task<PagingResponse<SplitTypeResponse>> SearchAsync([FromQuery]SearchRequest request)
        {
            return await _splitTypeLogic.SearchAsync(request);
        }

        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(int), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPut(ApiPaths.SplitTypeUpdate)]
        public async Task<int> UpdateAsync(SplitTypeRequest request)
        {
            return await _splitTypeLogic.UpdateAsync(request);
        }
    }
}
