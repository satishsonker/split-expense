using SplitExpense.Models.Base;

namespace SplitExpense.Models.DTO
{
    public class UserGroupMappingResponse:BaseResponseModel
    {
        public int GroupId { get; set; }
        public int AddedUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int AddedByUserId { get; set; }
        public DateTime AddedAt { get; set; }
        public UserResponse AddedUser { get; set; }
        public string AddedByUser { get; set; }
        public string GroupName { get; set; }
    }
}
