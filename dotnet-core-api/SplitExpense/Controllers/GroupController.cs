using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitExpense.Logic;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DTO;
using SplitExpense.SharedResource;

namespace SplitExpense.Controllers
{
    [Authorize]
    [ApiController]
    public class GroupController(IGroupLogic groupLogic) : ControllerBase
    {
        private readonly IGroupLogic _groupLogic=groupLogic;

        [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost(ApiPaths.GroupCreate)]
        [Consumes("multipart/form-data")]
        public async Task<GroupResponse> CreateAsync(
            [FromForm] string name,
            [FromForm] string? icon,
             IFormFile? image,
            [FromForm] List<int>? members,
            [FromForm] int? groupTypeId,
            [FromForm] bool EnableGroupDate,
            [FromForm] bool EnableSettleUpReminders,
            [FromForm] bool EnableBalanceAlert,
            [FromForm] decimal? MaxGroupBudget,
            [FromForm] DateTime? StartDate,
            [FromForm] DateTime? EndDate)
        {
            var request = new GroupRequest
            {
                Name = name,
                Icon = icon,
                GroupImage = image,
                Members = members ?? [],
                GroupTypeId = groupTypeId,
                GroupDetail = new GroupDetailRequest()
                {
                    EnableBalanceAlert = EnableBalanceAlert,
                    EnableGroupDate = EnableGroupDate,
                    EnableSettleUpReminders = EnableSettleUpReminders,
                    MaxGroupBudget = MaxGroupBudget,
                    StartDate = StartDate,
                    EndDate = EndDate
                }
            };
            
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
        [Consumes("multipart/form-data")]
        public async Task<int> UpdateAsync(
            [FromForm] int id,
            [FromForm] string name,
            [FromForm] string? icon,
            IFormFile? image,
            [FromForm] List<int>? members,
            [FromForm] int? groupTypeId,
            [FromForm] bool EnableGroupDate,
            [FromForm] bool EnableSettleUpReminders,
            [FromForm] bool EnableBalanceAlert,
            [FromForm] decimal? MaxGroupBudget,
            [FromForm] DateTime? StartDate,
            [FromForm] DateTime? EndDate)
        {
            var request = new GroupRequest
            {
                Id = id,
                Name = name,
                Icon = icon,
                GroupImage = image,
                Members = members ?? [],
                GroupTypeId = groupTypeId,
                GroupDetail = new GroupDetailRequest()
                {
                    EnableBalanceAlert = EnableBalanceAlert,
                    EnableGroupDate = EnableGroupDate,
                    EnableSettleUpReminders = EnableSettleUpReminders,
                    MaxGroupBudget = MaxGroupBudget,
                    StartDate = StartDate,
                    EndDate = EndDate
                }
            };
            
            return await _groupLogic.UpdateAsync(request);
        }

        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpDelete(ApiPaths.GroupDelete)]
        public async Task<bool> DeleteAsync([FromRoute] int id)
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

        [ProducesResponseType(typeof(PagingResponse<GroupResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet(ApiPaths.GroupGetAll)]
        public async Task<PagingResponse<GroupResponse>> GetAllAsync([FromQuery] PagingRequest request)
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
