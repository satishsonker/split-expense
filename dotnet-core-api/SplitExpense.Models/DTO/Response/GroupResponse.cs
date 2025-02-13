using SplitExpense.Models.Base;
using SplitExpense.Models.DTO;

namespace SplitExpense.Models
{
    public class GroupResponse: BaseResponseModel
    {
        public string? Name { get; set; }
        public string? Icon { get; set; }
        public int CreatedBy { get; set; }
        public string? ImagePath { get; set; }
        public string? ThumbImagePath { get; set; }
        public List<UserGroupMappingResponse>? Members { get; set; }
        public GroupTypeResponse? GroupType { get; set; }
        public GroupDetailResponse? GroupDetail { get; set; }
    }
}
