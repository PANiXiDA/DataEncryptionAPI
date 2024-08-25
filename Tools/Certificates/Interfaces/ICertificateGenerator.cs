using System.Security.Cryptography.X509Certificates;

namespace Tools.Certificates.Interfaces
{
    public interface ICertificateGenerator
    {
        X509Certificate2 CreateCertificate(string subjectName, string password, string path, string publicKeyPath, string privateKeyPath);
    }
}
