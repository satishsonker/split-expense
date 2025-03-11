using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DTO;

namespace SplitExpense.Data.Factory
{
    public interface IExpenseActivityFactory
    {
        Task<ExpenseActivity> CreateAsync(ExpenseActivity activity);
        Task<PagingResponse<ExpenseActivity>> GetAllAsync(PagingRequest request);
        Task<PagingResponse<ExpenseActivity>> SearchAsync(SearchRequest request);
        Task<bool> CreateRangeAsync(List<ExpenseActivity> request);
    }
} 