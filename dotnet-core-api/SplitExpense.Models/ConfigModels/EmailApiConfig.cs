namespace SplitExpense.Models.ConfigModels
{
    public class EmailApiConfig
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string InboxId { get; set; } // Optional, used by some providers
        public string From { get; set; }
    }
}
