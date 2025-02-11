using SplitExpense.Models.Base;
using SplitExpense.Models.DTO;

namespace SplitExpense.Models
{
    public class GroupResponse: BaseResponseModel
    {
        public string Name { get; set; }
        public int CreatedBy { get; set; }
        public List<UserGroupMappingResponse> Members { get; set; }
    }
}
