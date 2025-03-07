using System.ComponentModel.DataAnnotations.Schema;

namespace SplitExpense.Models.DbModels
{
    public class UserGroupMapping : BaseDbModels
    {
        public int FriendId { get; set; }
        public int GroupId { get; set; }

        [ForeignKey(nameof(FriendId))]
        public User? AddedUser { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public User? AddedByUser { get; set; }

        [ForeignKey(nameof(GroupId))]
        public ExpenseGroup? Group { get; set; }
    }
}
