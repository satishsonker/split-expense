using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
