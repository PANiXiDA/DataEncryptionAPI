using UI.Areas.Public.Models;

namespace DataEncryption.Models.Responses
{
    public class LoginResponse
    {
        public UserModel? User {  get; set; }
        public TokenModel? Token { get; set; }
    }
}
