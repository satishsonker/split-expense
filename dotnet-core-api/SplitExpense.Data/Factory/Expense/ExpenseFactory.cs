
using SplitExpense.Models;
using SplitExpense.SharedResource;

namespace SplitExpense.Data.Factory
{
    public class ExpenseFactory(SplitExpenseDbContext context) : IExpenseFactory
    {
        private readonly SplitExpenseDbContext _context = context;
        public async Task<Expense> AddExpense(Expense request, Dictionary<int, (decimal value, decimal adjustedAmount)> splitValues)
        {
            request.ExpenseShares = request.CalculateShares(splitValues);
            _context.Expenses.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<bool> DeleteExpense(int expenseId)
        {
            var expense = _context.Expenses.Find(expenseId);
            if (expense == null) throw new Exception("Expense not found");

            _context.Expenses.Remove(expense);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Expense> UpdateExpense(Expense request, Dictionary<int, (decimal value, decimal adjustedAmount)> splitValues)
        {
            var expense = _context.Expenses.Find(request.Id);
            if (expense == null) throw new Exception("Expense not found");

            expense.Description = request.Description;
            expense.Amount = request.Amount;
            expense.SplitTypeId = request.SplitTypeId;
            expense.ExpenseShares = expense.CalculateShares(splitValues);

            _context.Expenses.Update(expense);
            await _context.SaveChangesAsync();

            return expense;
        }
    }
}
