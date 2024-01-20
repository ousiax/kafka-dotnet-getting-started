using System.Text.Json.Serialization;

namespace Json.Serdes.Models
{
    public class User
    {
        [JsonRequired]
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonRequired]
        [JsonPropertyName("item")]
        public string Item { get; set; } = null!;
    }
}