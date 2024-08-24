using Common.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Tools.AsymmetricEncryption
{
    public class RsaEncryption
    {
        private readonly RSAParameters _privateKey;
        private readonly RSAParameters _publicKey;

        public RsaEncryption(IOptions<SharedConfiguration> sharedConfiguration)
        {
            _privateKey = ParseRsaParameters(sharedConfiguration.Value.RsaEncryptionConfiguration.PrivateKey, "private");
            _publicKey = ParseRsaParameters(sharedConfiguration.Value.RsaEncryptionConfiguration.PublicKey, "public");
        }

        public static void GeneratePrivateAndPublicKeys()
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.KeySize = 2048;

                var privateKey = rsa.ExportParameters(true);
                var publicKey = rsa.ExportParameters(false);

                SaveKeyToJson("appsettings.json", privateKey, publicKey);
            }
        }

        private static void SaveKeyToJson(string fileName, RSAParameters privateKey, RSAParameters publicKey)
        {
            var json = File.ReadAllText(fileName);
            var jsonObject = JsonDocument.Parse(json).RootElement.Clone();

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
                {
                    writer.WriteStartObject();
                    foreach (var property in jsonObject.EnumerateObject())
                    {
                        if (property.NameEquals("SharedConfiguration"))
                        {
                            writer.WriteStartObject(property.Name);

                            foreach (var sharedProp in property.Value.EnumerateObject())
                            {
                                if (sharedProp.NameEquals("RsaEncryptionConfiguration"))
                                {
                                    writer.WriteStartObject(sharedProp.Name);

                                    writer.WriteString("PrivateKey",
                                        $"{Convert.ToBase64String(privateKey.Modulus ?? Array.Empty<byte>())};" +
                                        $"{Convert.ToBase64String(privateKey.Exponent ?? Array.Empty<byte>())};" +
                                        $"{Convert.ToBase64String(privateKey.D ?? Array.Empty<byte>())};" +
                                        $"{Convert.ToBase64String(privateKey.P ?? Array.Empty<byte>())};" +
                                        $"{Convert.ToBase64String(privateKey.Q ?? Array.Empty<byte>())};" +
                                        $"{Convert.ToBase64String(privateKey.DP ?? Array.Empty<byte>())};" +
                                        $"{Convert.ToBase64String(privateKey.DQ ?? Array.Empty<byte>())};" +
                                        $"{Convert.ToBase64String(privateKey.InverseQ ?? Array.Empty<byte>())}");

                                    writer.WriteString("PublicKey",
                                        $"{Convert.ToBase64String(publicKey.Modulus ?? Array.Empty<byte>())};" +
                                        $"{Convert.ToBase64String(publicKey.Exponent ?? Array.Empty<byte>())}");

                                    writer.WriteEndObject();
                                }
                                else
                                {
                                    sharedProp.WriteTo(writer);
                                }
                            }

                            writer.WriteEndObject();
                        }
                        else
                        {
                            property.WriteTo(writer);
                        }
                    }
                    writer.WriteEndObject();
                }

                File.WriteAllText(fileName, Encoding.UTF8.GetString(stream.ToArray()));
            }
        }

        private RSAParameters ParseRsaParameters(string keyString, string keyType)
        {
            var keyParts = keyString.Split(';');

            if (keyParts.Length != 8 && keyParts.Length != 2)
            {
                throw new ArgumentException("Invalid RSA key format.");
            }
            var rsaParameters = keyType switch
            {
                "private" => new RSAParameters
                {
                    Modulus = Convert.FromBase64String(keyParts[0]),
                    Exponent = Convert.FromBase64String(keyParts[1]),
                    D = Convert.FromBase64String(keyParts[2]),
                    P = Convert.FromBase64String(keyParts[3]),
                    Q = Convert.FromBase64String(keyParts[4]),
                    DP = Convert.FromBase64String(keyParts[5]),
                    DQ = Convert.FromBase64String(keyParts[6]),
                    InverseQ = Convert.FromBase64String(keyParts[7]),
                },
                "public" => new RSAParameters
                {
                    Modulus = Convert.FromBase64String(keyParts[0]),
                    Exponent = Convert.FromBase64String(keyParts[1])
                },
                _ => new RSAParameters()
            };

            return rsaParameters;
        }

        public string Encrypt(object obj)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(_publicKey);

                string json = JsonSerializer.Serialize(obj);
                byte[] data = Encoding.UTF8.GetBytes(json);

                byte[] encryptData = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);

                return Convert.ToBase64String(encryptData);
            }
        }

        public string Decrypt(string encryptedData)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(_privateKey);

                byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

                byte[] decryptData = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);

                string json = Encoding.UTF8.GetString(decryptData);
                return json;
            }
        }
    }
}
