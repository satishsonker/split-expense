namespace SplitExpense.Models
{
    public class ErrorLog : BaseDbModels
    {
        public string? Stacks { get; set; }
        public string? Message { get; set; }
        public string? Exception { get; set; }
        public string? InnerException { get; set; }
        public string? InnerMessage { get; set; }
        public bool Resolved { get; set; }
    }
}
