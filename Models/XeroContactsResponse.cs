using System.Text.Json.Serialization;

namespace XeroDemo.Models
{
    public class XeroContactsResponse
    {
        [JsonPropertyName("Contacts")]
        public List<XeroContactDto> Contacts { get; set; } = [];
    }
}
