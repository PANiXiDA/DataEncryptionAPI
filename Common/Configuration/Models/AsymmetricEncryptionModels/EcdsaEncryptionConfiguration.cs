namespace Common.Configuration.Models.AsymmetricEncryptionModels
{
    public class EcdsaEncryptionConfiguration
    {
        public string PrivateKey { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
    }
}
