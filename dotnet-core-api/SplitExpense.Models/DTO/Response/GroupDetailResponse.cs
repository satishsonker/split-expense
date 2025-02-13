using SplitExpense.Models.Base;

namespace SplitExpense.Models.DTO
{
    public class GroupDetailResponse : BaseResponseModel
    {
        public int GroupId { get; set; }
        public bool EnableGroupDate { get; set; }
        public bool EnableSettleUpReminders { get; set; }
        public bool EnableBalanceAlert { get; set; }
        public decimal? MaxGroupBudget { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
