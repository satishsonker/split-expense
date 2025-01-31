using System.ComponentModel.DataAnnotations;

namespace SplitExpense.EmailManagement.Models
{
    public class BaseDbModel
    {

        [Key]
        public int Id { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
