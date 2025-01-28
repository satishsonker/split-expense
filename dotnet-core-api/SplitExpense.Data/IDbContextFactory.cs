namespace SplitExpense.Data
{
    public interface IDbContextFactory
    {
        SplitExpenseDbContext CreateDbContext();
    }
}
