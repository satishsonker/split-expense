namespace SplitExpense.Models.DTO
{
    public class EmailTemplateRequest
    {
        public int Id { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public string TemplateCode { get; set; }
        public bool IsHtml { get; set; }
    }
}
