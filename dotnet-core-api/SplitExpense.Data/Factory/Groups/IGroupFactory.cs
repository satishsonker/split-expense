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
        Task<UserGroupMapping?> GetUserGroupMappingAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<UserGroupMapping> AddFriendInGroupAsync(AddFriendInGroupRequest request);
        Task<PagingResponse<UserGroupMapping>> SearchAsync(SearchRequest request);
    }
}
