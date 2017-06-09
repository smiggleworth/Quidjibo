using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Quidjibo.Protectors
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
            using (var algorithm = Aes.Create())
            using (var encryptor = algorithm.CreateEncryptor(_key, algorithm.IV))
            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                await cryptoStream.WriteAsync(payload, 0, payload.Length, cancellationToken);
                cryptoStream.FlushFinalBlock();
                var encryptedPayload = memoryStream.ToArray();
                var outputBuffer = new byte[encryptedPayload.Length + algorithm.IV.Length];
                Buffer.BlockCopy(algorithm.IV, 0, outputBuffer, 0, algorithm.IV.Length);
                Buffer.BlockCopy(encryptedPayload, 0, outputBuffer, algorithm.IV.Length, encryptedPayload.Length);
                return outputBuffer;
            }
        }

        public async Task<byte[]> UnprotectAysnc(byte[] payload, CancellationToken cancellationToken)
        {
            var iv = new byte[16];
            Buffer.BlockCopy(payload, 0, iv, 0, iv.Length);
            using (var algorithm = Aes.Create())
            using (var decryptor = algorithm.CreateDecryptor(_key, iv))
            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
            {
                await cryptoStream.WriteAsync(payload, iv.Length, payload.Length - iv.Length, cancellationToken);
                cryptoStream.FlushFinalBlock();
                return memoryStream.ToArray();
            }
        }
    }
}