using SplitExpense.Models;

namespace SplitExpense.Logic
{
    public interface ICrudLogic<Tin,Tout> where Tin : class where Tout : class
    {
        Task<Tout> CreateAsync(Tin request);
        Task<int> UpdateAsync(Tin request);
        Task<Tout> GetAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Tout>> GetAllAsync(PagingRequest request);
        Task<IEnumerable<Tout>> SearchAsync(SearchRequest request);
    }
}
