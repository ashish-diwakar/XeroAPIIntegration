using System.Text.Json.Serialization;

namespace XeroDemo.Models
{
    public class ContactDto
    {
        [JsonPropertyName("ContactID")]
        public string ContactId { get; set; } = string.Empty;
    }
}
