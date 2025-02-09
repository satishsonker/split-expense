using SplitExpense.Models.DTO;

namespace SplitExpense.Logic
{
    public interface IExpenseLogic
    {
        Task<ExpenseResponse> AddExpense(ExpenseRequest request);
        Task<ExpenseResponse> UpdateExpense(ExpenseRequest request);
        Task<bool> DeleteExpense(int expenseId);
    }
}
