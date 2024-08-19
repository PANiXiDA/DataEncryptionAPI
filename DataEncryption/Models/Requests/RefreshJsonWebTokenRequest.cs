namespace DataEncryption.Models.Requests
{
    public class RefreshJsonWebTokenRequest
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
