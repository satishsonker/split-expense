using Microsoft.EntityFrameworkCore;
using SplitExpense.Models.DbModels;

namespace SplitExpense.Data.Factory
{
    public class ExpenseFactory(SplitExpenseDbContext context) : IExpenseFactory
    {
        private readonly SplitExpenseDbContext _context = context;
        public async Task<Expense> AddExpense(Expense request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Expenses.Add(request);
                await _context.SaveChangesAsync();

                // Add expense shares if provided
                if (request.ExpenseShares != null && request.ExpenseShares.Any())
                {
                    foreach (var share in request.ExpenseShares)
                    {
                        share.ExpenseId = request.Id;
                    }
                    await _context.ExpenseShares.AddRangeAsync(request.ExpenseShares);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return request;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteExpense(int expenseId)
        {
            var expense = _context.Expenses.Find(expenseId);
            if (expense == null) throw new Exception("Expense not found");

            _context.Expenses.Remove(expense);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Expense> UpdateExpense(Expense request)
        {
            var expense = _context.Expenses.Find(request.Id) ?? throw new Exception("Expense not found");
            expense.Description = request.Description;
            expense.Amount = request.Amount;
            expense.SplitTypeId = request.SplitTypeId;

            _context.Expenses.Update(expense);
            await _context.SaveChangesAsync();

            return expense;
        }
    }
}
