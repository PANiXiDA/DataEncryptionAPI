using Newtonsoft.Json;

namespace DataEncryption.Models.Requests
{
    public class GetUserRequest : BaseRequest
    {
        public string Token { get; set; } = string.Empty;
    }
}
