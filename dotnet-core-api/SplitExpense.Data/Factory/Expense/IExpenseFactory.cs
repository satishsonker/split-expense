using SplitExpense.Models;

namespace SplitExpense.Data.Factory
{
    public interface IExpenseFactory
    {
        Task<Expense> AddExpense(Expense request, Dictionary<int, (decimal value, decimal adjustedAmount)> splitValues); 
        Task<Expense> UpdateExpense(Expense request, Dictionary<int, (decimal value, decimal adjustedAmount)> splitValues); 
        Task<bool> DeleteExpense(int expenseId);
    }
}
