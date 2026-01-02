using SplitExpense.Models.Base;

namespace SplitExpense.Models.DTO
{
    public class RemoveFriendFromGroup: BaseRequestModel
    {
        public required int GroupId { get; set; }
        public required int FriendId { get; set; }
    }
}
