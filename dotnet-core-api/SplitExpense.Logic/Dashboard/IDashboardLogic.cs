using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DTO;

namespace SplitExpense.Logic
{
    public interface IDashboardLogic
    {
        Task<DashboardSummaryResponse> GetDashboardSummaryAsync(int userId);
        Task<PagingResponse<ExpenseResponse>> GetExpensesYouOweAsync(int userId, PagingRequest request);
        Task<PagingResponse<ExpenseResponse>> GetExpensesYouAreOwedAsync(int userId, PagingRequest request);
        Task<List<MemberBalanceResponse>> GetMemberBalancesAsync(int userId);
        Task<bool> SettleAmountAsync(int fromUserId, int toUserId, decimal amount, string? description);
        Task<List<MonthlyExpenseSummaryResponse>> GetMonthlyExpenseSummaryAsync(int userId, int months);
    }
}

