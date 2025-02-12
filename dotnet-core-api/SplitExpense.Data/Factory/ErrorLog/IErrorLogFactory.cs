using SplitExpense.Models.DbModels;

namespace SplitExpense.Data.Factory
{
    public interface IErrorLogFactory
    {
        Task<int> LogErrorAsync(ErrorLog errorLog);
        int LogError(ErrorLog errorLog);
    }
}
