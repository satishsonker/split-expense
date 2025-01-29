using System.Text.Json.Serialization;

namespace SplitExpense.Models
{
    public class BaseErrorResponse
    {
        public string? ErrorResponseType { get; set; }
        public string? Message { get; set; }

        [JsonIgnore]
        public object? Errors { get; set; }
    }
}
