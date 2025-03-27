using SplitExpense.Models.Base;

namespace SplitExpense.Models.DTO
{
    public class AddFriendInGroupRequest : BaseRequestModel
    {
        public required int GroupId { get; set; }
        public required List<int> FriendIds { get; set; }
    }
}
