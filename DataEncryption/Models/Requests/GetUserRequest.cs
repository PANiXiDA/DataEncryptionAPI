using Newtonsoft.Json;

namespace DataEncryption.Models.Requests
{
    public class GetUserRequest : BaseRequest
    {
        [JsonProperty("user_token")]
        public string Token { get; set; } = string.Empty;
    }
}
