using Newtonsoft.Json;

namespace DataEncryption.Models.Requests
{
    public class GetTokenRequest : BaseRequest
    {
        [JsonProperty("user_id")]
        public int Id { get; set; }
    }
}
