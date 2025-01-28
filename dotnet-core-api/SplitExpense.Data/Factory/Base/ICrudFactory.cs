using SplitExpense.Models;

namespace SplitExpense.Data.Factory
{
    public interface ICrudFactory<T> where T : class
    {
        Task<T> CreateAsync(T request);
        Task<int> UpdateAsync(T request);
        Task<T> GetAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(PagingRequest request);
        Task<IEnumerable<T>> SearchAsync(SearchRequest request);
    }
}
