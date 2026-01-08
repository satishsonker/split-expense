using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DbModels;
using SplitExpense.Models.DTO;

namespace SplitExpense.Data.Factory
{
    public interface IDashboardFactory
    {
        Task<DashboardSummaryResponse> GetDashboardSummaryAsync(int userId);
        Task<PagingResponse<Expense>> GetExpensesYouOweAsync(int userId, PagingRequest request);
        Task<PagingResponse<Expense>> GetExpensesYouAreOwedAsync(int userId, PagingRequest request);
        Task<List<MemberBalanceResponse>> GetMemberBalancesAsync(int userId);
        Task<bool> SettleAmountAsync(int fromUserId, int toUserId, decimal amount, string? description);
        Task<List<MonthlyExpenseSummaryResponse>> GetMonthlyExpenseSummaryAsync(int userId, int months);
    }
}

