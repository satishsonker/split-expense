using SplitExpense.Models.DbModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace SplitExpense.Models
{
    public class Group : BaseDbModels
    {
        public required int UserId { get; set; }
        public required string Name { get; set; }
        public string? Icon { get; set; }
        public string? ImagePath { get; set; }
        public int? GroupTypeId { get; set; }

        [ForeignKey(nameof(GroupTypeId))]
        public GroupType? GroupType { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        public List<UserGroupMapping> Members { get; set; }
    }
}
