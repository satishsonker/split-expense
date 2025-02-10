using SplitExpense.Models.DTO;
using System.ComponentModel.DataAnnotations.Schema;

namespace SplitExpense.Models
{
    public class Group : BaseDbModels
    {
        public required int UserId { get; set; }
        public required string Name { get; set; }
        public required string? Icon { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public List<UserGroupMapping> Members { get; set; }
    }
}
