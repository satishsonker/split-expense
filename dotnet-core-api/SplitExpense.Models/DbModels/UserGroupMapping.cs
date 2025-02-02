using System.ComponentModel.DataAnnotations.Schema;

namespace SplitExpense.Models
{
    public class UserGroupMapping : BaseDbModels
    {
        public int UserId { get; set; }
        public int GroupId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [ForeignKey(nameof(GroupId))]
        public Group? Group { get; set; }
    }
}
