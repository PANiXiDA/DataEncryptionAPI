using Newtonsoft.Json;

namespace DataEncryption.Models.Requests
{
    public class GetTokenRequest : BaseRequest
    {
        public int Id { get; set; }
    }
}
