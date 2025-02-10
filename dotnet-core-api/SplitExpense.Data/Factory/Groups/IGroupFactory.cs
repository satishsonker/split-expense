using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DTO;
namespace SplitExpense.Data.Factory
{
    public interface IGroupFactory
    {
        Task<PagingResponse<Group>> GetAllAsync(PagingRequest request);
        Task<Group> CreateAsync(Group request);
        Task<int> UpdateAsync(Group request);
        Task<Group?> GetAsync(int id);
        Task<UserGroupMapping?> GetUserGroupMappingAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<UserGroupMapping> AddFriendInGroupAsync(AddFriendInGroupRequest request);
        Task<PagingResponse<UserGroupMapping>> SearchAsync(SearchRequest request);
    }
}
