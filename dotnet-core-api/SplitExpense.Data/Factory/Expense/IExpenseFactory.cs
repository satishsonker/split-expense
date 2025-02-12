using SplitExpense.Models.DbModels;

namespace SplitExpense.Data.Factory
{
    public interface IExpenseFactory
    {
        Task<Expense> AddExpense(Expense request); 
        Task<Expense> UpdateExpense(Expense request); 
        Task<bool> DeleteExpense(int expenseId);
    }
}
