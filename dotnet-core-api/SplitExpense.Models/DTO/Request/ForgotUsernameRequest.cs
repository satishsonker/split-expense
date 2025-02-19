using System.ComponentModel.DataAnnotations;
namespace SplitExpense.Models.DTO
{
    public class ForgotUsernameRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}