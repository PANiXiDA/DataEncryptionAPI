namespace Tools.JsonWebTokenEncryption.Models
{
    public class JsonWebTokenData
    {
        public string AccessToken {  get; set; } = string.Empty;
        public string RefreshToken {  get; set; } = string.Empty;
        public DateTime CreatedAt {  get; set; }
        public DateTime AccessTokenExpiresAt {  get; set; }
        public DateTime RefreshTokenExpiresAt { get; set; }
    }
}
