using Tools.Certificates.Generators;
using Tools.Certificates.Interfaces;

namespace Tools.Certificates
{
    public class CertificateFactory
    {
        private readonly string _typeEncryption;

        public CertificateFactory(string typeEncryption)
        {
            _typeEncryption = typeEncryption;
        }

        public ICertificateGenerator GetCertificateGenerator()
        {
            return _typeEncryption.ToLower() switch
            {
                "rsa" => new RsaCertificateGenerator(),
                "ecdsa" => new EcdsaCertificateGenerator(),
                _ => throw new NotSupportedException($"Encryption type {_typeEncryption} is not supported")
            };
        }
    }
}
