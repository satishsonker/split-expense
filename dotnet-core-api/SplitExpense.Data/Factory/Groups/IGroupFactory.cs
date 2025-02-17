using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DbModels;
using SplitExpense.Models.DTO;
namespace SplitExpense.Data.Factory
{
    public interface IGroupFactory
    {
        Task<PagingResponse<Group>> GetAllAsync(PagingRequest request);
        Task<Group> CreateAsync(Group request,List<int> members);
        Task<int> UpdateAsync(Group request, List<int> members);
        Task<Group?> GetAsync(int id);
        Task<UserGroupMapping?> GetUserGroupMappingAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<UserGroupMapping> AddFriendInGroupAsync(AddFriendInGroupRequest request);
        Task<PagingResponse<UserGroupMapping>> SearchAsync(SearchRequest request);
    }
}
