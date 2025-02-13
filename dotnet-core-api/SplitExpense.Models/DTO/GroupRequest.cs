using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SplitExpense.Models
{
    public class GroupRequest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public string? Icon { get; set; }
        
        // For image upload
        public IFormFile? Image { get; set; }
        
        public List<int> Members { get; set; } = [];
        public int? GroupTypeId { get; set; }
        public GroupDetailRequest? GroupDetail { get; set; }
    }
} 