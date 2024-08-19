using Common.Configuration.Models;

namespace Common.Configuration
{
    public class SharedConfiguration
    {
        public string BaseUrl { get; set; } = string.Empty;
        public AesEncryptionConfiguration AesEncryptionConfiguration { get; set; } = new AesEncryptionConfiguration();
        public JsonWebTokenConfiguration JsonWebTokenConfiguration { get; set; } = new JsonWebTokenConfiguration();
        public int? CleanupIntervalHours { get; set; }

        public void UpdateSharedConfiguration(int? cleanupIntervalHours)
        {
            CleanupIntervalHours = cleanupIntervalHours;
        }
    }
}
