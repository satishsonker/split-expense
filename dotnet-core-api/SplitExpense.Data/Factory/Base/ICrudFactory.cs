using SplitExpense.Models;
using SplitExpense.Models.Common;

namespace SplitExpense.Data.Factory
{
    public interface ICrudFactory<T> where T : class
    {
        Task<T> CreateAsync(T request);
        Task<int> UpdateAsync(T request);
        Task<T?> GetAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<PagingResponse<T>> GetAllAsync(PagingRequest request);
        Task<PagingResponse<T>> SearchAsync(SearchRequest request);
    }
}
