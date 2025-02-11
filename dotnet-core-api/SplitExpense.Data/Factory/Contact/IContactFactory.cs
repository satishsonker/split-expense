using SplitExpense.Models;
using SplitExpense.Models.Common;

namespace SplitExpense.Data.Factory
{
    public interface IContactFactory
    {
        Task<Contact> CreateAsync(User request);
        Task<bool> AddInContactListAsync(int contactUserId);
        Task<int> UpdateAsync(Contact request);
        Task<Contact?> GetAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<List<User>> SearchUser(string search);
        Task<PagingResponse<Contact>> GetAllAsync(PagingRequest request);
        Task<PagingResponse<Contact>> SearchAsync(SearchRequest request);
    }
}
