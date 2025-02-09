using Microsoft.AspNetCore.Mvc;
using SplitExpense.Logic;
using SplitExpense.Models.Common;
using SplitExpense.Models;
using SplitExpense.Models.DTO;
using SplitExpense.SharedResource;

namespace SplitExpense.Controllers
{
    [ApiController]
    public class ExpenseController(IExpenseLogic expenseLogic) : ControllerBase
    {
        private readonly IExpenseLogic _expenseLogic = expenseLogic;

        [ProducesResponseType(typeof(ExpenseResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost(ApiPaths.ExpenseCreate)]
        public async Task<ExpenseResponse> AddExpense([FromBody] ExpenseRequest request)
        {
            return await _expenseLogic.AddExpense(request);
        }

        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpDelete(ApiPaths.ExpenseDelete)]
        public async Task<bool> DeleteExpense([FromRoute]int expenseId)
        {
            return await _expenseLogic.DeleteExpense(expenseId);
        }

        [ProducesResponseType(typeof(ExpenseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPut(ApiPaths.ExpenseUpdate)]
        public async Task<ExpenseResponse> UpdateExpense([FromBody]ExpenseRequest request)
        {
            return await _expenseLogic.UpdateExpense(request);
        }
    }
}
