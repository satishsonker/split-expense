namespace SplitExpense.Models.DTO
{
    public class GroupTypeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? GroupNameSuggestion { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Icon { get; set; } = string.Empty;
    }
} 