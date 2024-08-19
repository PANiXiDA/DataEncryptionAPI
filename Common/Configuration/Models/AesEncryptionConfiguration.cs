namespace Common.Configuration.Models
{
    public class AesEncryptionConfiguration
    {
        public string SecretKey { get; set; } = string.Empty;
        public int TokenLifeTimeDays { get; set; }
    }
}
