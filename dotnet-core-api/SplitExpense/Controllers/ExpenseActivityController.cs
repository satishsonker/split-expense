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
    public class ExpenseActivityController : ControllerBase
    {
        private readonly IExpenseActivityLogic _expenseActivityLogic;

        public ExpenseActivityController(IExpenseActivityLogic expenseActivityLogic)
        {
            _expenseActivityLogic = expenseActivityLogic;
        }

        [HttpPost(ApiPaths.ExpenseActivityCreate)]
        public async Task<ActionResult<ExpenseActivityResponse>> CreateAsync([FromBody] ExpenseActivityRequest request)
        {
            return await _expenseActivityLogic.CreateAsync(request);
        }

        [HttpGet(ApiPaths.ExpenseActivityGetAll)]
        public async Task<ActionResult<PagingResponse<ExpenseActivityResponse>>> GetAllAsync([FromQuery] PagingRequest request)
        {
            return await _expenseActivityLogic.GetAllAsync(request);
        }

        [HttpGet(ApiPaths.ExpenseActivitySearch)]
        public async Task<ActionResult<PagingResponse<ExpenseActivityResponse>>> SearchAsync([FromQuery] SearchRequest request)
        {
            return await _expenseActivityLogic.SearchAsync(request);
        }
    }
} 