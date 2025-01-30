using SplitExpense.Data.BaseModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace SplitExpense.Data.DbModels
{
    public class Group : BaseDbModels
    {
        public required int UserId { get; set; }
        public required string Name { get; set; }
        public required string? Icon { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
