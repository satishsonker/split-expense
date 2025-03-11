using Microsoft.EntityFrameworkCore;
using SplitExpense.ExceptionManagement.Exceptions;
using SplitExpense.Logger;
using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.SharedResource;
using System.Diagnostics;

namespace SplitExpense.Data.Factory
{
    public class ExpenseActivityFactory(SplitExpenseDbContext context, ISplitExpenseLogger logger) : IExpenseActivityFactory
    {
        private readonly SplitExpenseDbContext _context = context;
        private readonly ISplitExpenseLogger _logger = logger;

        public async Task<ExpenseActivity> CreateAsync(ExpenseActivity activity)
        {
            try
            {
                var entity = await _context.ExpenseActivities.AddAsync(activity);
                await _context.SaveChangesAsync();
                return entity.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating expense activity");
                throw;
            }
        }

        public async Task<bool> CreateRangeAsync(List<ExpenseActivity> request)
        {
            try
            {
                await _context.ExpenseActivities.AddRangeAsync(request);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating expense activity");
                throw;
            }
        }

        public async Task<PagingResponse<ExpenseActivity>> GetAllAsync(PagingRequest request)
        {
            try
            {
                var query = _context.ExpenseActivities
                    .Where(x => !x.IsDeleted)
                    .OrderByDescending(x => x.CreatedAt)
                    .AsQueryable();

                return new PagingResponse<ExpenseActivity>
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
                _logger.LogError(ex, "Error getting all expense activities");
                throw;
            }
        }

        public async Task<PagingResponse<ExpenseActivity>> SearchAsync(SearchRequest request)
        {
            try
            {
                var query = _context.ExpenseActivities
                    .Where(x => !x.IsDeleted && 
                        x.Activity.Contains(request.SearchTerm))
                    .OrderBy(x => x.Activity)
                    .AsQueryable();

                return new PagingResponse<ExpenseActivity>
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
                _logger.LogError(ex, "Error searching expense activities");
                throw;
            }
        }
    }
} 