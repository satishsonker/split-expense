using SplitExpense.SharedResource;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SplitExpense.Models
{
    public class Expense:BaseDbModels
    {

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [ForeignKey("User")]
        public int PaidByUserId { get; set; }
        public virtual User PaidByUser { get; set; }

        [ForeignKey("Group")]
        public int? GroupId { get; set; }
        public virtual Group Group { get; set; }

        [ForeignKey("SplitType")]
        public int SplitTypeId { get; set; }
        public virtual SplitType SplitType { get; set; }

        public virtual ICollection<ExpenseShare> ExpenseShares { get; set; }

        // Method to calculate shares dynamically
        public List<ExpenseShare> CalculateShares(Dictionary<int, (decimal value, decimal adjustedAmount)> splitValues)
        {
            List<ExpenseShare> shares = [];
            decimal totalShares = splitValues.Values.Sum(v => v.value);

            foreach (var entry in splitValues)
            {
                shares.Add(new ExpenseShare
                {
                    ExpenseId = this.Id,
                    UserId = entry.Key,
                    Percentage = SplitTypeId == (int)SharedResource.SplitType.Percentage ? entry.Value.value : 0,
                    Shares = SplitTypeId == (int)SharedResource.SplitType.Shares ? entry.Value.value : 0,
                    ExactAmount = SplitTypeId == (int)SharedResource.SplitType.ExactAmount ? entry.Value.value : 0,
                    AdjustedAmount = SplitTypeId == (int)SharedResource.SplitType.Adjustment ? entry.Value.adjustedAmount : 0,
                    AmountOwed = SplitTypeId switch
                    {
                        (int)SharedResource.SplitType.Equal => Amount / splitValues.Count,
                        (int)SharedResource.SplitType.Percentage => Amount * (entry.Value.value / 100),
                        (int)SharedResource.SplitType.ExactAmount => entry.Value.value,
                        (int)SharedResource.SplitType.Shares => (entry.Value.value / totalShares) * Amount,
                        (int)SharedResource.SplitType.Adjustment => entry.Value.adjustedAmount,
                        _ => throw new InvalidOperationException("Invalid Split Type")
                    }
                });
            }
            return shares;
        }
    }

}
