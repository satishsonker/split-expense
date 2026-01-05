using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DTO;

namespace SplitExpense.Logic
{
    public interface IExpenseLogic
    {
        Task<ExpenseResponse> AddExpense(ExpenseRequest request);
        Task<ExpenseResponse> UpdateExpense(ExpenseRequest request);
        Task<bool> DeleteExpense(int expenseId);
        Task<PagingResponse<ExpenseResponse>> GetAllAsync(PagingRequest request);
        Task<PagingResponse<ExpenseResponse>> SearchAsync(SearchRequest request);
    }
}
