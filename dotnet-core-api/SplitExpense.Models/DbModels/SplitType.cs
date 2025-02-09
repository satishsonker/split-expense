namespace SplitExpense.Models
{
    public class SplitType:BaseDbModels
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string Icon { get; set; }
    }
}
