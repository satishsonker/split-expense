using System.ComponentModel.DataAnnotations.Schema;

namespace SplitExpense.Models
{
    public class Friend:BaseDbModels
    {
        public int UserId { get; set; }
        public int FriendId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [ForeignKey(nameof(FriendId))]
        public User? UserFriend { get; set; }
    }
}
