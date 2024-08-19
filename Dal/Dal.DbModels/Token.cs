namespace Dal.DbModels
{
    public partial class Token
    {
        public int Id { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int UserId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime AccessTokenExpiresAt { get; set; }
        public DateTime RefreshTokenExpiresAt { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }

        public virtual User? User { get; set; }
    }
}
