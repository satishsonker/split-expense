using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DTO;

namespace SplitExpense.Logic
{
    public interface IGroupTypeLogic
    {
        Task<GroupTypeResponse> CreateAsync(GroupTypeRequest request);
        Task<int> UpdateAsync(GroupTypeRequest request);
        Task<GroupTypeResponse?> GetAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<PagingResponse<GroupTypeResponse>> GetAllAsync(PagingRequest request);
        Task<PagingResponse<GroupTypeResponse>> SearchAsync(SearchRequest request);
    }
} 