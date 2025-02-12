using Microsoft.EntityFrameworkCore;
using SplitExpense.Models.DbModels;
using SplitExpense.Logger;

namespace SplitExpense.Data.Factory
{
    public class ErrorLogFactory(SplitExpenseDbContext context,ISplitExpenseLogger logger) : IErrorLogFactory
    {
        private readonly SplitExpenseDbContext _context=context;
        private readonly ISplitExpenseLogger _logger=logger;

        public int LogError(ErrorLog error)
        {
            try
            {
                _context.ErrorLogs.Add(error);
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new DbUpdateException(ex.Message);
            }
        }

        public async Task<int> LogErrorAsync(ErrorLog errorLog)
        {
            try
            {
                _context.ErrorLogs.Add(errorLog);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new DbUpdateException(ex.Message);
            }
        }
    }
}
