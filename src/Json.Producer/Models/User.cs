using Newtonsoft.Json;

namespace Json.Serdes.Models
{
    public class User
    {

        [JsonRequired] // use Newtonsoft.Json annotations
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonRequired]
        [JsonProperty("item")]
        public string Item { get; set; } = null!;
    }
}