using System.ComponentModel.DataAnnotations;

namespace SplitExpense.Models.DTO
{
    public class GroupTypeRequest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
        
        public string Icon { get; set; } = string.Empty;
    }
} 