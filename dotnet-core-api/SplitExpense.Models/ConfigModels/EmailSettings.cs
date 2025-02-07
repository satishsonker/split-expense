namespace SplitExpense.Models.ConfigModels
{
    public class EmailSettings
    {
        public string Provider { get; set; } // "SMTP" or "API"
        public string DefaultSmtp { get; set; }
        public string DefaultApi { get; set; }
        public Dictionary<string, SmtpConfig> SmtpProviders { get; set; }
        public Dictionary<string, EmailApiConfig> ApiProviders { get; set; }
    }
}
