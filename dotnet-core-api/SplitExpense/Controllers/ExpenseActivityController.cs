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
    public class ExpenseActivityController(IExpenseActivityLogic expenseActivityLogic) : ControllerBase
    {
        private readonly IExpenseActivityLogic _expenseActivityLogic = expenseActivityLogic;

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