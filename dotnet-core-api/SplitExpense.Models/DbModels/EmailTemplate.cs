namespace SplitExpense.Models
{
    public class EmailTemplate : BaseDbModels
    {
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public string TemplateCode { get; set; }
        public bool IsHtml { get; set; }
    }
}
