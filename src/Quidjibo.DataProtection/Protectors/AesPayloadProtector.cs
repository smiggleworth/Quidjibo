using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Protectors;

namespace Quidjibo.DataProtection.Protectors
{
    public class AesPayloadProtector : IPayloadProtector
    {
        private readonly byte[] _key;

        public AesPayloadProtector(byte[] key)
        {
            _key = key;
        }

        public async Task<byte[]> ProtectAsync(byte[] payload, CancellationToken cancellationToken)
        {
            using (var aes = Aes.Create())
            using (var crypto = aes.CreateEncryptor(_key, aes.IV))
            using (var stream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(stream, crypto, CryptoStreamMode.Write))
            {
                await cryptoStream.WriteAsync(payload, 0, payload.Length, cancellationToken);
                cryptoStream.FlushFinalBlock();
                var encryptedPayload = stream.ToArray();
                var outputBuffer = new byte[encryptedPayload.Length + aes.IV.Length];
                Buffer.BlockCopy(aes.IV, 0, outputBuffer, 0, aes.IV.Length);
                Buffer.BlockCopy(encryptedPayload, 0, outputBuffer, aes.IV.Length, encryptedPayload.Length);
                return outputBuffer;
            }
        }

        public async Task<byte[]> UnprotectAysnc(byte[] payload, CancellationToken cancellationToken)
        {
            var iv = new byte[128 / 8];
            Buffer.BlockCopy(payload, 0, iv, 0, iv.Length);
            using (var aes = Aes.Create())
            using (var crypto = aes.CreateDecryptor(_key, iv))
            using (var stream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(stream, crypto, CryptoStreamMode.Write))
            {
                await cryptoStream.WriteAsync(payload, iv.Length, payload.Length - iv.Length, cancellationToken);
                cryptoStream.FlushFinalBlock();
                return stream.ToArray();
            }
        }
    }
}