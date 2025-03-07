using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SplitExpense.Models.DbModels
{
    public class Expense:BaseDbModels
    {

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;

        public int PaidByUserId { get; set; }

        public int? GroupId { get; set; }

        public int SplitTypeId { get; set; }

        public List<ExpenseShare> ExpenseShares { get; set; }

        [ForeignKey(nameof(SplitTypeId))]
        public virtual SplitType SplitType { get; set; }

        [ForeignKey(nameof(GroupId))]
        public ExpenseGroup Group { get; set; }

        [ForeignKey(nameof(PaidByUserId))]
        public User PaidByUser { get; set; }
        // Method to calculate shares dynamically
        //public List<ExpenseShare> CalculateShares(Dictionary<int, (decimal value, decimal adjustedAmount)> splitValues)
        //{
        //    List<ExpenseShare> shares = [];
        //    decimal totalShares = splitValues.Values.Sum(v => v.value);

        //    foreach (var entry in splitValues)
        //    {
        //        shares.Add(new ExpenseShare
        //        {
        //            ExpenseId = this.Id,
        //            UserId = entry.Key,
        //            Percentage = SplitTypeId == (int)SplitTypeEnum.Percentage ? entry.Value.value : 0,
        //            Shares = SplitTypeId == (int)SplitTypeEnum.Shares ? entry.Value.value : 0,
        //            ExactAmount = SplitTypeId == (int)SplitTypeEnum.ExactAmount ? entry.Value.value : 0,
        //            AdjustedAmount = SplitTypeId == (int)SplitTypeEnum.Adjustment ? entry.Value.adjustedAmount : 0,
        //            AmountOwed = SplitTypeId switch
        //            {
        //                (int)SplitTypeEnum.Equal => Amount / splitValues.Count,
        //                (int)SplitTypeEnum.Percentage => Amount * (entry.Value.value / 100),
        //                (int)SplitTypeEnum.ExactAmount => entry.Value.value,
        //                (int)SplitTypeEnum.Shares => (entry.Value.value / totalShares) * Amount,
        //                (int)SplitTypeEnum.Adjustment => entry.Value.adjustedAmount,
        //                _ => throw new InvalidOperationException("Invalid Split Type")
        //            }
        //        });
        //    }
        //    return shares;
        //}
    }

}
