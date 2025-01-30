using SplitExpense.Models.Base;

namespace SplitExpense.Models.DTO
{
    public class UserGroupMappingResponse:BaseResponseModel
    {
        public string? GroupName { get; set; }
        public string? GroupIcon { get; set; }
        public string? CreatedByUser { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? AddedByUser { get; set; }
        public int AddedByUserId { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
