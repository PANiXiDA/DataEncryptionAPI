using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Tools.SymmetricEncryption.Models;
using Common.Configuration;

namespace Tools.SymmetricEncryption
{
    public class AesEncryption
    {
        private readonly byte[] _key;
        private readonly TimeSpan _tokenLifeTime;

        public AesEncryption(IOptions<SharedConfiguration> sharedConfiguration)
        {
            _key = Encoding.UTF8.GetBytes(sharedConfiguration.Value.AesEncryptionConfiguration.SecretKey);
            _tokenLifeTime = TimeSpan.FromDays(sharedConfiguration.Value.AesEncryptionConfiguration.TokenLifeTimeDays);
        }


        public string Encrypt<T>(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            string json = JsonSerializer.Serialize(obj);

            var tokenData = new TokenData
            {
                Timestamp = DateTime.UtcNow,
                Data = json
            };

            string tokenJson = JsonSerializer.Serialize(tokenData);

            byte[] plainText = Encoding.UTF8.GetBytes(tokenJson);
            byte[] cipherText = EncryptData(plainText);

            return Convert.ToBase64String(cipherText);
        }

        private byte[] EncryptData(byte[] plainText)
        {
            using Aes aes = Aes.Create();
            aes.GenerateIV();

            aes.Key = _key;
            byte[] iv = aes.IV;

            using ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using MemoryStream ms = new MemoryStream();

            ms.Write(iv, 0, iv.Length);

            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                cs.Write(plainText, 0, plainText.Length);
            }

            return ms.ToArray();
        }


        public T Decrypt<T>(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));

            byte[] cipherBytes = Convert.FromBase64String(token);

            byte[] plainText = DecryptData(cipherBytes);

            string tokenJson = Encoding.UTF8.GetString(plainText);
            var tokenData = JsonSerializer.Deserialize<TokenData>(tokenJson);

            if (tokenData == null)
                throw new InvalidOperationException("Invalid token data.");

            if (DateTime.UtcNow - tokenData.Timestamp > _tokenLifeTime)
                throw new InvalidOperationException("Token has expired.");

            T? result = JsonSerializer.Deserialize<T>(tokenData.Data);
            if (result == null)
                throw new InvalidOperationException("Deserialized object is null.");

            return result;
        }

        private byte[] DecryptData(byte[] cipherText)
        {
            using Aes aes = Aes.Create();
            aes.Key = _key;

            byte[] iv = new byte[16];
            Array.Copy(cipherText, 0, iv, 0, iv.Length);
            aes.IV = iv;

            byte[] actualCipherText = new byte[cipherText.Length - iv.Length];
            Array.Copy(cipherText, iv.Length, actualCipherText, 0, actualCipherText.Length);

            using ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using MemoryStream ms = new MemoryStream(actualCipherText);
            using CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using MemoryStream resultStream = new MemoryStream();
            cs.CopyTo(resultStream);

            return resultStream.ToArray();
        }
    }
}

