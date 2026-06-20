using System.Text.Json.Serialization;

namespace XeroDemo.Models
{
    public class XeroContactDto
    {
        [JsonPropertyName("ContactID")]
        public Guid ContactId { get; set; }

        [JsonPropertyName("Name")]
        public string? Name { get; set; }

        [JsonPropertyName("EmailAddress")]
        public string? EmailAddress { get; set; }

        [JsonPropertyName("ContactNumber")]
        public string? ContactNumber { get; set; }

        [JsonPropertyName("IsSupplier")]
        public bool IsSupplier { get; set; }

        [JsonPropertyName("IsCustomer")]
        public bool IsCustomer { get; set; }
    }
}
