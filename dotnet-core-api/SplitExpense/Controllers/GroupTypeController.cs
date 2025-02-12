using Microsoft.AspNetCore.Mvc;
using SplitExpense.Logic;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DTO;
using SplitExpense.SharedResource;

namespace SplitExpense.Controllers
{
    [ApiController]
    public class GroupTypeController(IGroupTypeLogic groupTypeLogic) : ControllerBase
    {
        private readonly IGroupTypeLogic _groupTypeLogic = groupTypeLogic;

        [ProducesResponseType(typeof(GroupTypeResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost(ApiPaths.GroupTypeCreate)]
        public async Task<GroupTypeResponse> CreateAsync([FromBody] GroupTypeRequest request)
        {
            return await _groupTypeLogic.CreateAsync(request);
        }

        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpDelete(ApiPaths.GroupTypeDelete)]
        public async Task<bool> DeleteAsync([FromRoute] int id)
        {
            return await _groupTypeLogic.DeleteAsync(id);
        }

        [ProducesResponseType(typeof(PagingResponse<GroupTypeResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet(ApiPaths.GroupTypeGetAll)]
        public async Task<PagingResponse<GroupTypeResponse>> GetAllAsync([FromQuery] PagingRequest request)
        {
            return await _groupTypeLogic.GetAllAsync(request);
        }

        [ProducesResponseType(typeof(GroupTypeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet(ApiPaths.GroupTypeGetById)]
        public async Task<GroupTypeResponse> GetAsync([FromRoute] int id)
        {
            return await _groupTypeLogic.GetAsync(id);
        }

        [ProducesResponseType(typeof(PagingResponse<GroupTypeResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet(ApiPaths.GroupTypeSearch)]
        public async Task<PagingResponse<GroupTypeResponse>> SearchAsync([FromQuery] SearchRequest request)
        {
            return await _groupTypeLogic.SearchAsync(request);
        }

        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPut(ApiPaths.GroupTypeUpdate)]
        public async Task<int> UpdateAsync([FromBody] GroupTypeRequest request)
        {
            return await _groupTypeLogic.UpdateAsync(request);
        }
    }
} 