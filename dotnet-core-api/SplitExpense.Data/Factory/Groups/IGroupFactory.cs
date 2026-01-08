using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DbModels;
using SplitExpense.Models.DTO;
namespace SplitExpense.Data.Factory
{
    public interface IGroupFactory
    {
        Task<PagingResponse<ExpenseGroup>> GetAllAsync(PagingRequest request);
        Task<List<ExpenseGroup>> GetRecentGroups();
        Task<ExpenseGroup> CreateAsync(ExpenseGroup request,List<int> members);
        Task<int> UpdateAsync(ExpenseGroup request, List<int> members);
        Task<ExpenseGroup?> GetAsync(int id);
        Task<List<UserGroupMapping>> GetUserGroupMappingAsync(List<int> ids);
        Task<bool> DeleteAsync(int id);
        Task<List<UserGroupMapping>> AddFriendInGroupAsync(AddFriendInGroupRequest request);
        Task<bool> RemoveFriendInGroupAsync(RemoveFriendFromGroup request);
        Task<PagingResponse<UserGroupMapping>> SearchAsync(SearchRequest request);
        Task<GroupSummaryResponse> GetGroupSummaryAsync(int groupId, int userId);
        Task<Dictionary<int, GroupSummaryResponse>> GetGroupsSummaryAsync(List<int> groupIds, int userId);
        Task<GroupExpenseBreakdownResponse> GetGroupExpenseBreakdownAsync(int groupId, int userId);
    }
}
