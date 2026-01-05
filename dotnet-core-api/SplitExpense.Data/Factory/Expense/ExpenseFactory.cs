using Microsoft.EntityFrameworkCore;
using SplitExpense.Models;
using SplitExpense.Models.Common;
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

        public async Task<PagingResponse<Expense>> GetAllAsync(PagingRequest request)
        {
            try
            {
                var query = _context.Expenses
                    .Where(x => !x.IsDeleted)
                    .Include(x => x.ExpenseShares)
                    .OrderByDescending(x => x.ExpenseDate)
                    .ThenByDescending(x => x.CreatedAt)
                    .AsQueryable();

                return new PagingResponse<Expense>
                {
                    Data = await query
                        .Skip((request.PageNo - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToListAsync(),
                    RecordCounts = await query.CountAsync(),
                    PageSize = request.PageSize,
                    PageNo = request.PageNo
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting all expenses", ex);
            }
        }

        public async Task<PagingResponse<Expense>> SearchAsync(SearchRequest request)
        {
            try
            {
                var query = _context.Expenses
                    .Where(x => !x.IsDeleted &&
                        (x.Description.Contains(request.SearchTerm) ||
                         x.Amount.ToString().Contains(request.SearchTerm)))
                    .Include(x => x.ExpenseShares)
                    .OrderByDescending(x => x.ExpenseDate)
                    .ThenByDescending(x => x.CreatedAt)
                    .AsQueryable();

                return new PagingResponse<Expense>
                {
                    Data = await query
                        .Skip((request.PageNo - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToListAsync(),
                    RecordCounts = await query.CountAsync(),
                    PageSize = request.PageSize,
                    PageNo = request.PageNo
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error searching expenses", ex);
            }
        }
    }
}
