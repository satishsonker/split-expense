namespace SplitExpense.Models.DbModels
{
    public class GroupType : BaseDbModels
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? GroupNameSuggestion { get; set; }
    }
}
