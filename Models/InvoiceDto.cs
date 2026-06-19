using System.Text.Json.Serialization;

namespace XeroDemo.Models
{
    public class InvoiceDto
    {
        [JsonPropertyName("Type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("Contact")]
        public ContactDto Contact { get; set; } = new();

        [JsonPropertyName("LineItems")]
        public List<LineItemDto> LineItems { get; set; } = new();

        [JsonPropertyName("Date")]
        public string Date { get; set; } = string.Empty;

        [JsonPropertyName("DueDate")]
        public string DueDate { get; set; } = string.Empty;

        [JsonPropertyName("Status")]
        public string Status { get; set; } = string.Empty;
    }
}
