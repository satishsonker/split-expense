using SplitExpense.Models;
using SplitExpense.Models.Common;
using SplitExpense.Models.DTO;
using SplitExpense.SharedResource;

namespace SplitExpense.Logic
{
    public interface IExpenseActivityLogic
    {
        Task<ExpenseActivityResponse> CreateAsync(ExpenseActivityRequest request);
        Task<bool> CreateRangeAsync(List<ExpenseActivityRequest> request);
        Task<PagingResponse<ExpenseActivityResponse>> GetAllAsync(PagingRequest request);
        Task<PagingResponse<ExpenseActivityResponse>> SearchAsync(SearchRequest request);
        string GetActivityMessage(ExpenseActivityTypeEnum activityType,Dictionary<string,string> data);
    }
} 