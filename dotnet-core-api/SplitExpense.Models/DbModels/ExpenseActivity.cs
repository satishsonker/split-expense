using SplitExpense.Models.DbModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace SplitExpense.Models
{
    [Table("ExpenseActivities")]
    public class ExpenseActivity:BaseDbModels
    {
        public int ActivityType { get; set; }
        public string Activity { get; set; }
        public string Icon { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
