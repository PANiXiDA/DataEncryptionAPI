namespace Entities
{
    public class Token
    {
        public int Id { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public User User {  get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt {  get; set; }
        public DateTime AccessTokenExpiresAt { get; set; }
        public DateTime RefreshTokenExpiresAt { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }

        public Token(
            int id,
            string accessToken,
            string refreshToken,
            User user,
            bool isActive,
            DateTime createdAt,
            DateTime accessTokenExpiresAt,
            DateTime refreshTokenExpiresAt,
            string? ipAddress,
            string? userAgent)
        {
            Id = id;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            User = user;
            IsActive = isActive;
            CreatedAt = createdAt;
            AccessTokenExpiresAt = accessTokenExpiresAt;
            RefreshTokenExpiresAt = refreshTokenExpiresAt;
            IpAddress = ipAddress;
            UserAgent = userAgent;
        }
    }
}
