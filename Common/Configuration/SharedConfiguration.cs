using Common.Configuration.Models;
using Common.Configuration.Models.AsymmetricEncryptionModels;

namespace Common.Configuration
{
    public class SharedConfiguration
    {
        public string BaseUrl { get; set; } = string.Empty;
        public AesEncryptionConfiguration AesEncryptionConfiguration { get; set; } = new AesEncryptionConfiguration();
        public JsonWebTokenConfiguration JsonWebTokenConfiguration { get; set; } = new JsonWebTokenConfiguration();
        public RsaEncryptionConfiguration RsaEncryptionConfiguration { get; set; } = new RsaEncryptionConfiguration();
        public static int? CleanupIntervalHours { get; set; }
        public CertificateConfiguration CertificateConfiguration { get; set; } = new CertificateConfiguration();

        public static void UpdateSharedConfiguration(int? cleanupIntervalHours = null)
        {
            CleanupIntervalHours = cleanupIntervalHours;
        }
    }
}
