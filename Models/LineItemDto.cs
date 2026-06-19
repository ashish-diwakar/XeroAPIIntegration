using System.Text.Json.Serialization;

namespace XeroDemo.Models
{
    public class LineItemDto
    {
        [JsonPropertyName("Description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("Quantity")]
        public decimal Quantity { get; set; }

        [JsonPropertyName("UnitAmount")]
        public decimal UnitAmount { get; set; }

        [JsonPropertyName("AccountCode")]
        public string AccountCode { get; set; } = string.Empty;
    }
}
