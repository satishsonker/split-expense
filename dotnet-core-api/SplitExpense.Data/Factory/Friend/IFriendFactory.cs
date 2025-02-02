using SplitExpense.Models;
using SplitExpense.Models.Common;

namespace SplitExpense.Data.Factory
{
    public interface IFriendFactory
    {
        Task<Friend> AddFriend(Friend request);
        Task<Friend> AddFriend(User request);
        Task<PagingResponse<Friend>> GetAllFriend(PagingRequest request);
        Task<PagingResponse<Friend>> SearchFriend(SearchRequest request);
        Task<bool> DeleteFriend(int friendId);
    }
}
