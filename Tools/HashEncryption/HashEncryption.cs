using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Tools.HashEncryption
{
    public class HashEncryption
    {
        public string GetHash<T>(T obj) 
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            string salt = "1";
            //string salt = GenerateSalt();

            string jsonData = JsonSerializer.Serialize(obj);

            using var sha512 = SHA512.Create();
            byte[] data = Encoding.UTF8.GetBytes(jsonData + salt);
            byte[] hashBytes = sha512.ComputeHash(data);

            return string.Concat(hashBytes.Select(item => item.ToString("x2")));
        }

        private string GenerateSalt(int size = 32)
        {
            byte[] saltBytes = new byte[size];
            RandomNumberGenerator.Fill(saltBytes);

            return Convert.ToBase64String(saltBytes);
        }
    }
}
