using Microsoft.AspNetCore.Mvc;
using SplitExpense.Logic;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DTO;
using SplitExpense.SharedResource;

namespace SplitExpense.Controllers
{

    [ApiController]
    public class GroupController(IGroupLogic groupLogic) : ControllerBase
    {
        private readonly IGroupLogic _groupLogic=groupLogic;

        [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost(ApiPaths.GroupCreate)]
        public async Task<GroupResponse> CreateAsync([FromBody] GroupRequest request)
        {
            return await _groupLogic.CreateAsync(request);
        }
        
        [ProducesResponseType(typeof(bool), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost(ApiPaths.GroupAddUser)]
        public async Task<bool> AddFriendInGroupAsync([FromBody] AddFriendInGroupRequest request)
        {
            return await _groupLogic.AddFriendInGroupAsync(request);
        }

        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPut(ApiPaths.GroupUpdate)]
        public async Task<int> UpdateAsync([FromBody] GroupRequest request)
        {
            return await _groupLogic.UpdateAsync(request);
        }

        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpDelete(ApiPaths.GroupDelete)]
        public async Task<bool> UpdateAsync([FromRoute] int id)
        {
            return await _groupLogic.DeleteAsync(id);
        }

        [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet(ApiPaths.GroupGetById)]
        public async Task<GroupResponse> GetAsync([FromRoute] int id)
        {
            return await _groupLogic.GetAsync(id);
        }

        [ProducesResponseType(typeof(PagingResponse<UserGroupMappingResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet(ApiPaths.GroupGetAll)]
        public async Task<PagingResponse<UserGroupMappingResponse>> GetAllAsync([FromQuery] PagingRequest request)
        {
            return await _groupLogic.GetAllAsync(request);
        }

        [ProducesResponseType(typeof(PagingResponse<UserGroupMappingResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet(ApiPaths.GroupSearch)]
        public async Task<PagingResponse<UserGroupMappingResponse>> SearchAsync([FromQuery] SearchRequest request)
        {
            return await _groupLogic.SearchAsync(request);
        }
    }
}
