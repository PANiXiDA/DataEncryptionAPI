using Common.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace Tools.Certificates
{
    public class Certificate
    {
        private readonly string _certificatePath;
        private readonly string _certificatePassword;
        private readonly string _domain;
        private readonly string _typeEncryption;
        private readonly string _publicKeyPath;
        private readonly string _privateKeyPath;

        public Certificate(SharedConfiguration sharedConfiguration)
        {
            _certificatePath = sharedConfiguration.CertificateConfiguration.Path;
            _certificatePassword = sharedConfiguration.CertificateConfiguration.Password;
            _domain = sharedConfiguration.CertificateConfiguration.Domain;
            _typeEncryption = sharedConfiguration.CertificateConfiguration.TypeEncryption;
            _publicKeyPath = sharedConfiguration.CertificateConfiguration.PublicKeyPath;
            _privateKeyPath = sharedConfiguration.CertificateConfiguration.PrivateKeyPath;
        }

        public X509Certificate2 Create()
        {
            var factory = new CertificateFactory(_typeEncryption);
            var generator = factory.GetCertificateGenerator();

            var certificate = generator.CreateCertificate(
                _domain,
                _certificatePassword,
                _certificatePath,
                _publicKeyPath,
                _privateKeyPath);

            return certificate;
        }
    }
}

