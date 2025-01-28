using Microsoft.EntityFrameworkCore;

namespace SplitExpense.Data
{
    public class DbContextFactory(DbContextOptions<SplitExpenseDbContext> options) : IDbContextFactory
    {
        private readonly DbContextOptions<SplitExpenseDbContext> _options = options;

        public SplitExpenseDbContext CreateDbContext()
        {
            return new SplitExpenseDbContext(_options);
        }
    }
}
