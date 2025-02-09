using SplitExpense.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace SplitExpense.Models.DTO
{
    public class ExpenseRequest : BaseRequestModel
    {
        public string Description { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;

        public int PaidByUserId { get; set; }

        public int? GroupId { get; set; }

        public int SplitTypeId { get; set; }

        public List<ExpenseShareRequest> ExpenseShares { get; set; }
    }
}
