using Entities;

namespace UI.Areas.Public.Models
{
    public class TokenModel
    {
        public int Id { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public UserModel? User { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime AccessTokenExpiresAt { get; set; }
        public DateTime RefreshTokenExpiresAt { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }

        public static TokenModel? FromEntity(Token obj)
        {
            return obj == null ? null : new TokenModel
            {
                Id = obj.Id,
                AccessToken = obj.AccessToken,
                RefreshToken = obj.RefreshToken,
                User = UserModel.FromEntity(obj.User),
                IsActive = obj.IsActive,
                CreatedAt = obj.CreatedAt,
                AccessTokenExpiresAt = obj.AccessTokenExpiresAt,
                RefreshTokenExpiresAt = obj.RefreshTokenExpiresAt,
                IpAddress = obj.IpAddress,
                UserAgent = obj.UserAgent
            };
        }

        public static Token? ToEntity(TokenModel obj)
        {
            return obj == null ? null : new Token(
                obj.Id,
                obj.AccessToken,
                obj.RefreshToken,
                UserModel.ToEntity(obj.User!) ?? throw new ArgumentNullException(nameof(obj.User)),
                obj.IsActive,
                obj.CreatedAt,
                obj.AccessTokenExpiresAt,
                obj.RefreshTokenExpiresAt,
                obj.IpAddress,
                obj.UserAgent);
        }

        public static List<TokenModel> FromEntitiesList(IEnumerable<Token> list)
        {
            return list?.Select(FromEntity).Where(x => x != null).Cast<TokenModel>().ToList() ?? new List<TokenModel>();
        }

        public static List<Token> ToEntitiesList(IEnumerable<TokenModel> list)
        {
            return list?.Select(ToEntity).Where(x => x != null).Cast<Token>().ToList() ?? new List<Token>();
        }
    }
}
