using System.ComponentModel.DataAnnotations.Schema;

namespace SplitExpense.Data.DbModels
{
    public class UserGroupMapping : BaseModels.BaseDbModels
    {
        public int UserId { get; set; }
        public int GroupId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [ForeignKey(nameof(GroupId))]
        public Group? Group { get; set; }
    }
}
