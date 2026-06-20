using System.Text.Json.Serialization;

namespace XeroDemo.Models
{
    public class XeroConfigurationDto
    {
        [JsonPropertyName("ClientId")]
        public string ClientId { get; set; } = string.Empty;

        [JsonPropertyName("ClientSecret")]
        public string ClientSecret { get; set; } = string.Empty;

        [JsonPropertyName("TokenServiceEndPoint")]
        public string TokenServiceEndPoint { get; set; } = string.Empty;

        [JsonPropertyName("ContactServiceEndPoint")]
        public string ContactServiceEndPoint { get; set; } = string.Empty;

        [JsonPropertyName("InvoiceServiceEndPoint")]
        public string InvoiceServiceEndPoint { get; set; } = string.Empty;        
    }
}
