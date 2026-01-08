using AutoMapper;
using SplitExpense.Data.Factory;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DTO;

namespace SplitExpense.Logic
{
    public class DashboardLogic(IDashboardFactory dashboardFactory, IMapper mapper) : IDashboardLogic
    {
        private readonly IDashboardFactory _dashboardFactory = dashboardFactory;
        private readonly IMapper _mapper = mapper;

        public async Task<DashboardSummaryResponse> GetDashboardSummaryAsync(int userId)
        {
            return await _dashboardFactory.GetDashboardSummaryAsync(userId);
        }

        public async Task<PagingResponse<ExpenseResponse>> GetExpensesYouOweAsync(int userId, PagingRequest request)
        {
            var result = await _dashboardFactory.GetExpensesYouOweAsync(userId, request);
            return _mapper.Map<PagingResponse<ExpenseResponse>>(result);
        }

        public async Task<PagingResponse<ExpenseResponse>> GetExpensesYouAreOwedAsync(int userId, PagingRequest request)
        {
            var result = await _dashboardFactory.GetExpensesYouAreOwedAsync(userId, request);
            return _mapper.Map<PagingResponse<ExpenseResponse>>(result);
        }

        public async Task<List<MemberBalanceResponse>> GetMemberBalancesAsync(int userId)
        {
            return await _dashboardFactory.GetMemberBalancesAsync(userId);
        }

        public async Task<bool> SettleAmountAsync(int fromUserId, int toUserId, decimal amount, string? description)
        {
            return await _dashboardFactory.SettleAmountAsync(fromUserId, toUserId, amount, description);
        }

        public async Task<List<MonthlyExpenseSummaryResponse>> GetMonthlyExpenseSummaryAsync(int userId, int months)
        {
            return await _dashboardFactory.GetMonthlyExpenseSummaryAsync(userId, months);
        }
    }
}

