using System.ComponentModel.DataAnnotations.Schema;

namespace SplitExpense.Models.DbModels
{
    public class Contact : BaseDbModels
    {

        public required int UserId { get; set; }
        public required int ContactId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [ForeignKey(nameof(ContactId))]
        public User ContactUser { get; set; }
    }
}
