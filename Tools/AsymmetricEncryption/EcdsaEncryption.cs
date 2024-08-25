using Common.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace Tools.AsymmetricEncryption
{
    public class EcdsaEncryption
    {
        private readonly string _privateKey;
        private readonly string _publicKey;
        private readonly byte[] _certificate;
        private readonly string _certificatePassword;

        public EcdsaEncryption(IOptions<SharedConfiguration> sharedConfiguration)
        {
            _privateKey = File.ReadAllText(sharedConfiguration.Value.CertificateConfiguration.PrivateKeyPath);
            _publicKey = File.ReadAllText(sharedConfiguration.Value.CertificateConfiguration.PublicKeyPath);
            _certificate = File.ReadAllBytes(sharedConfiguration.Value.CertificateConfiguration.Path);
            _certificatePassword = sharedConfiguration.Value.CertificateConfiguration.Password;
        }

        //public static void GenerateKeysAndSaveToPem()
        //{
        //    using (ECDsa ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256))
        //    {
        //        string privateKeyPem = ecdsa.ExportECPrivateKeyPem();
        //        File.WriteAllText("private_key.pem", privateKeyPem);

        //        string publicKeyPem = ecdsa.ExportSubjectPublicKeyInfoPem();
        //        File.WriteAllText("public_key.pem", publicKeyPem);
        //    }
        //}

        public string Sign<T>(T data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            string json = JsonSerializer.Serialize(data);
            byte[] dataBytes = Encoding.UTF8.GetBytes(json);
            string signedData = SignData(dataBytes);

            return signedData;
        }

        private string SignData(byte[] dataBytes)
        {
            using var crt = new X509Certificate2(_certificate, _certificatePassword);
            using (var ecdsa = ECDsa.Create())
            {
                ecdsa.ImportFromPem(_privateKey);

                var signer = new CmsSigner(new X509Certificate2(crt));
                var signedCms = new SignedCms(new ContentInfo(dataBytes), true);

                signedCms.ComputeSignature(signer);

                var signedData = signedCms.Encode();
                var testConv = Convert.ToBase64String(signedData);

                return Convert.ToBase64String(signedData);
            }
        }

        public bool VerifySignedData<T>(string signedData, out T data)
        {
            data = default;

            try
            {
                // Декодируем Base64 строку обратно в массив байтов
                byte[] signedDataBytes = Convert.FromBase64String(signedData);

                // Создаем объект SignedCms для анализа подписи
                var signedCms = new SignedCms();
                signedCms.Decode(signedDataBytes);

                // Импортируем публичный ключ из PEM
                using var ecdsa = ECDsa.Create();
                ecdsa.ImportFromPem(_publicKey);

                // Проверка подписи с использованием публичного ключа
                signedCms.CheckSignature(true);

                // Если подпись верна, получаем исходные данные
                byte[] originalDataBytes = signedCms.ContentInfo.Content;
                string originalDataJson = Encoding.UTF8.GetString(originalDataBytes);

                // Десериализация данных обратно в исходный объект
                data = JsonSerializer.Deserialize<T>(originalDataJson);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
