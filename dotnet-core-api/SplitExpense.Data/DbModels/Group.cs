using SplitExpense.Data.BaseModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace SplitExpense.Data.DbModels
{
    public class Group:BaseDbModels
    {
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public string Name { get; set; }
    }
}
