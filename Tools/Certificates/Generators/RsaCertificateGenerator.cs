using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Tools.Certificates.Interfaces;

namespace Tools.Certificates.Generators
{
    public class RsaCertificateGenerator : ICertificateGenerator
    {
        public X509Certificate2 CreateCertificate(
            string subjectName,
            string password,
            string path,
            string publicKeyPath,
            string privateKeyPath)
        {
            if (File.Exists(path))
            {
                return new X509Certificate2(path, password);
            }

            using (var rsa = RSA.Create(2048))
            {
                var distinguishedName = new X500DistinguishedName(subjectName);
                var request = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(1));

                File.WriteAllBytes(path, certificate.Export(X509ContentType.Pfx, password));
                return certificate;
            }
        }
    }
}
