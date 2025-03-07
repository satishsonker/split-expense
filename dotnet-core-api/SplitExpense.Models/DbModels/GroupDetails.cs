using System.ComponentModel.DataAnnotations.Schema;

namespace SplitExpense.Models.DbModels
{
    public class GroupDetail : BaseDbModels
    {
        public int GroupId { get; set; }
        public bool EnableGroupDate { get; set; }
        public bool EnableSettleUpReminders { get; set; }
        public bool EnableBalanceAlert { get; set; }
        public decimal? MaxGroupBudget { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [ForeignKey(nameof(GroupId))]
        public ExpenseGroup? Group { get; set; }


    }
}
