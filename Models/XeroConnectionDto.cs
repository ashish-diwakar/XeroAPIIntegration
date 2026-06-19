using System.Text.Json.Serialization;

namespace XeroDemo.Models
{
    public class XeroConnectionDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("tenantId")]
        public string TenantId { get; set; } = string.Empty;

        [JsonPropertyName("tenantName")]
        public string TenantName { get; set; } = string.Empty;

        [JsonPropertyName("tenantType")]
        public string TenantType { get; set; } = string.Empty; // e.g., "ORGANISATION"
    }
}
