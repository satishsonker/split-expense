using SplitExpense.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace SplitExpense.Models.DTO
{
    public class ExpenseResponse : BaseResponseModel
    {
        public string Description { get; set; }

        public decimal Amount { get; set; }

        public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;

        public int PaidByUserId { get; set; }

        public int? GroupId { get; set; }

        public int SplitTypeId { get; set; }

        public List<ExpenseShareResponse> ExpenseShares { get; set; } = [];
    }
}
