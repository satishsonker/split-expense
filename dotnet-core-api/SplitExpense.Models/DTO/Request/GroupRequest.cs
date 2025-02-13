using SplitExpense.Models.Base;

namespace SplitExpense.Models
{
    public class GroupRequest: BaseRequestModel
    {
        public string Name { get; set; }
        public List<int> Members { get; set; }
        public IFormFile MyProperty { get; set; }
    }
}
