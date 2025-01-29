using Microsoft.AspNetCore.Mvc;
using SplitExpense.Logic;
using SplitExpense.Models;
using SplitExpense.Models.Common;
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
        public async Task<GroupResponse> CreateAsync(GroupRequest request)
        {
            return await _groupLogic.CreateAsync(request);
        }
    }
}
