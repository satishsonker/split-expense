using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitExpense.Data
{
    public interface IDbContextFactory
    {
        SplitExpenseDbContext CreateDbContext();
    }
}
