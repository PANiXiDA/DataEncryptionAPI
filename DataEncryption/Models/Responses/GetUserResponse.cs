using UI.Areas.Public.Models;

namespace DataEncryption.Models.Responses
{
    public class GetUserResponse : BaseResponse
    {
        public UserModel? User { get; set; }
    }
}
