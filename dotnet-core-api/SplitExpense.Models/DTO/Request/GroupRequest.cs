using Microsoft.AspNetCore.Http;
using SplitExpense.Models.Base;
using SplitExpense.Models.DTO;

namespace SplitExpense.Models
{
    public class GroupRequest: BaseRequestModel
    {
        public required string Name { get; set; }
        public List<int>? Members { get; set; }
        public IFormFile? GroupImage { get; set; }
        public string? Icon { get; set; }
        public int? GroupTypeId { get; set; }
        public GroupDetailRequest? GroupDetail { get; set; }
    }
}
