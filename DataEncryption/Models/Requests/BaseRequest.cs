using Newtonsoft.Json;

namespace DataEncryption.Models.Requests
{
    public class BaseRequest
    {
        [JsonProperty("user_hash")]
        public string? Hash { get; set; }
    }
}
