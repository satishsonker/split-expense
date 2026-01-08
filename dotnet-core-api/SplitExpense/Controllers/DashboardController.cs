using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitExpense.Data.Services;
using SplitExpense.Logic;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DTO;
using SplitExpense.SharedResource;

namespace SplitExpense.Controllers
{
    [Authorize]
    [ApiController]
    public class DashboardController(IDashboardLogic dashboardLogic, IUserContextService userContext) : ControllerBase
    {
        private readonly IDashboardLogic _dashboardLogic = dashboardLogic;
        private readonly IUserContextService _userContext = userContext;

        [ProducesResponseType(typeof(DashboardSummaryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet(ApiPaths.DashboardGetSummary)]
        public async Task<DashboardSummaryResponse> GetSummaryAsync()
        {
            var userId = _userContext.GetUserId();
            return await _dashboardLogic.GetDashboardSummaryAsync(userId);
        }

        [ProducesResponseType(typeof(PagingResponse<ExpenseResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet(ApiPaths.DashboardGetExpensesYouOwe)]
        public async Task<PagingResponse<ExpenseResponse>> GetExpensesYouOweAsync([FromQuery] PagingRequest request)
        {
            var userId = _userContext.GetUserId();
            return await _dashboardLogic.GetExpensesYouOweAsync(userId, request);
        }

        [ProducesResponseType(typeof(PagingResponse<ExpenseResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet(ApiPaths.DashboardGetExpensesYouAreOwed)]
        public async Task<PagingResponse<ExpenseResponse>> GetExpensesYouAreOwedAsync([FromQuery] PagingRequest request)
        {
            var userId = _userContext.GetUserId();
            return await _dashboardLogic.GetExpensesYouAreOwedAsync(userId, request);
        }

        [ProducesResponseType(typeof(List<MemberBalanceResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet(ApiPaths.DashboardGetMemberBalances)]
        public async Task<List<MemberBalanceResponse>> GetMemberBalancesAsync()
        {
            var userId = _userContext.GetUserId();
            return await _dashboardLogic.GetMemberBalancesAsync(userId);
        }

        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost(ApiPaths.DashboardSettleAmount)]
        public async Task<bool> SettleAmountAsync([FromBody] SettleAmountRequest request)
        {
            var userId = _userContext.GetUserId();
            return await _dashboardLogic.SettleAmountAsync(userId, request.ToUserId, request.Amount, request.Description);
        }

        [ProducesResponseType(typeof(List<MonthlyExpenseSummaryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet(ApiPaths.DashboardGetMonthlySummary)]
        public async Task<List<MonthlyExpenseSummaryResponse>> GetMonthlyExpenseSummaryAsync([FromQuery] int months = 6)
        {
            var userId = _userContext.GetUserId();
            return await _dashboardLogic.GetMonthlyExpenseSummaryAsync(userId, months);
        }
    }
}

