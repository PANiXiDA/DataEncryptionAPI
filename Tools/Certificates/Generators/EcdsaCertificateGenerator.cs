using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Tools.Certificates.Interfaces;

namespace Tools.Certificates.Generators
{
    public class EcdsaCertificateGenerator : ICertificateGenerator
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

            using (var ecdsa = ECDsa.Create())
            {
                var distinguishedName = new X500DistinguishedName(subjectName);
                var request = new CertificateRequest(distinguishedName, ecdsa, HashAlgorithmName.SHA256);

                var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(1));
                File.WriteAllBytes(path, certificate.Export(X509ContentType.Pfx, password));

                var publicKeyPem = ecdsa.ExportSubjectPublicKeyInfoPem();
                File.WriteAllText(publicKeyPath, publicKeyPem);

                var privateKeyPem = ecdsa.ExportECPrivateKeyPem();
                File.WriteAllText(privateKeyPath, privateKeyPem);

                return certificate;
            }
        }
    }
}
