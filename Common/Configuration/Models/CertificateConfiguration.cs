namespace Common.Configuration.Models
{
    public class CertificateConfiguration
    {
        public string Path { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string TypeEncryption { get; set; } = string.Empty;
        public string PublicKeyPath {  get; set; } = string.Empty;
        public string PrivateKeyPath { get; set; } = string.Empty;
    }
}
