using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DbModels;

namespace SplitExpense.Data.Factory
{
    public interface IExpenseFactory
    {
        Task<Expense> AddExpense(Expense request); 
        Task<Expense> UpdateExpense(Expense request); 
        Task<bool> DeleteExpense(int expenseId);
        Task<PagingResponse<Expense>> GetAllAsync(PagingRequest request);
        Task<PagingResponse<Expense>> SearchAsync(SearchRequest request);
    }
}
