using Common.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

public class EcdsaEncryption
{
    private readonly byte[] _privateKey;
    private readonly ECPoint _publicKey;

    public EcdsaEncryption(IOptions<SharedConfiguration> sharedConfiguration)
    {
        _privateKey = HexStringToByteArray(sharedConfiguration.Value.EcdsaEncryptionConfiguration.PrivateKey);

        var publicKeyHex = sharedConfiguration.Value.EcdsaEncryptionConfiguration.PublicKey;
        _publicKey = new ECPoint
        {
            X = HexStringToByteArray(publicKeyHex.Substring(0, 64)),
            Y = HexStringToByteArray(publicKeyHex.Substring(64))
        };
    }

    //public byte[] SignData<T>(T data)
    //{
    //    using (var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256))
    //    {
    //        var parameters = new ECParameters
    //        {
    //            D = _privateKey,
    //            Q = _publicKey
    //        };
    //        ecdsa.ImportParameters(parameters);

    //        byte[] dataBytes;

    //        if (data is string strData)
    //        {
    //            dataBytes = Encoding.UTF8.GetBytes(strData);
    //        }
    //        else
    //        {
    //            dataBytes = ObjectToByteArray(data);
    //        }

    //        return ecdsa.SignData(dataBytes);
    //    }
    //}

    //public bool VerifyData<T>(T data, byte[] signature)
    //{
    //    using (var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256))
    //    {
    //        var parameters = new ECParameters
    //        {
    //            D = _privateKey,
    //            Q = _publicKey
    //        };
    //        ecdsa.ImportParameters(parameters);

    //        byte[] dataBytes;

    //        if (data is string strData)
    //        {
    //            dataBytes = Encoding.UTF8.GetBytes(strData);
    //        }
    //        else
    //        {
    //            dataBytes = ObjectToByteArray(data);
    //        }

    //        return ecdsa.VerifyData(dataBytes, signature);
    //    }
    //}

    private static byte[] HexStringToByteArray(string hex)
    {
        int numberChars = hex.Length;
        byte[] bytes = new byte[numberChars / 2];
        for (int i = 0; i < numberChars; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        return bytes;
    }

    //private static byte[] ObjectToByteArray<T>(T obj)
    //{
    //    using (var memoryStream = new System.IO.MemoryStream())
    //    {
    //        var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
    //        formatter.Serialize(memoryStream, obj);
    //        return memoryStream.ToArray();
    //    }
    //}
}
