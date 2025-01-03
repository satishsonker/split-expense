using System.ComponentModel.DataAnnotations;

namespace SplitExpense.Data.BaseModels
{
    public class BaseDbModels
    {
        [Key]
        public int Id { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
