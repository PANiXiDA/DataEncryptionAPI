using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Tools.Certificates
{
    public class Certificate
    {
        public static void Create()
        {
            if (!File.Exists("certificate.pfx"))
            {
                using (var rsa = RSA.Create(2048))
                {
                    var request = new CertificateRequest("cn=localhost", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                    var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(1));

                    var certData = certificate.Export(X509ContentType.Pfx, "password");

                    File.WriteAllBytes("certificate.pfx", certData);
                }
            }
        }
    }
}

