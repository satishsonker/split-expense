using SplitExpense.Models.Common;
using SplitExpense.Models;
using SplitExpense.Models.DTO;

namespace SplitExpense.Logic
{
    public interface IContactLogic
    {
        Task<ContactResponse> CreateAsync(UserRequest request);
        Task<bool> DeleteAsync(int id);
        Task<PagingResponse<ContactResponse>> GetAllAsync(PagingRequest request);
        Task<ContactResponse> GetAsync(int id);
        Task<PagingResponse<ContactResponse>> SearchAsync(SearchRequest request);
        Task<int> UpdateAsync(ContactRequest request);
        Task<List<UserResponse>> SearchUser(string search);
    }
}
