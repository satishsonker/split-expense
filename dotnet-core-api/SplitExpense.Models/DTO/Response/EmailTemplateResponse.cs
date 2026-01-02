namespace SplitExpense.Models.DTO
{
    public class EmailTemplateResponse
    {
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public string? TemplateCode { get; set; }
        public bool IsHtml { get; set; }
    }
}
