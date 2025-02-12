using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SplitExpense.Models.DbModels
{
    public class Transaction : BaseDbModels
    {
        public int FromUserId { get; set; }


        public int ToUserId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(FromUserId))]
        public User FromUser { get; set; }

        [ForeignKey(nameof(ToUserId))]
        public User ToUser { get; set; }
    }
}
