using SplitExpense.Data.DbModels;
using SplitExpense.Models.Common;
using SplitExpense.Models;
namespace SplitExpense.Data.Factory
{
    public interface IGroupFactory
    {
        Task<PagingResponse<UserGroupMapping>> GetAllAsync(PagingRequest request);
        Task<Group> CreateAsync(Group request);
        Task<int> UpdateAsync(Group request);
        Task<Group?> GetAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<bool> AddFriendInGroup(int id);
        Task<PagingResponse<UserGroupMapping>> SearchAsync(SearchRequest request);
    }
}
