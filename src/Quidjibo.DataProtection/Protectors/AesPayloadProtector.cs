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

        /*
         * Encrypted payload:
         * 
         * |--- IV (16 bytes) ---|--- MAC (32 bytes) ---|--- Ciphertext (n bytes) ---|
         * 
         */
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
                var mac = ComputeMac(encryptedPayload, 0 , encryptedPayload.Length);
                var outputBuffer = new byte[encryptedPayload.Length + aes.IV.Length + mac.Length];
                Buffer.BlockCopy(aes.IV, 0, outputBuffer, 0, aes.IV.Length);
                Buffer.BlockCopy(mac, 0, outputBuffer, aes.IV.Length, mac.Length);
                Buffer.BlockCopy(encryptedPayload, 0, outputBuffer, aes.IV.Length + mac.Length, encryptedPayload.Length);
                return outputBuffer;
            }
        }

        public async Task<byte[]> UnprotectAysnc(byte[] payload, CancellationToken cancellationToken)
        {
            var iv = new byte[128 / 8];
            Buffer.BlockCopy(payload, 0, iv, 0, iv.Length);
            var mac = new byte[32];
            Buffer.BlockCopy(payload, iv.Length, mac, 0, 32);
            byte[] plaintext;
            using (var aes = Aes.Create())
            using (var crypto = aes.CreateDecryptor(_key, iv))
            using (var stream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(stream, crypto, CryptoStreamMode.Write))
            {
                await cryptoStream.WriteAsync(payload, iv.Length + mac.Length, payload.Length - iv.Length - mac.Length, cancellationToken);
                cryptoStream.FlushFinalBlock();
                plaintext = stream.ToArray();
            }

            VerifyMac(payload, mac);

            return plaintext;
        }

        private byte[] ComputeMac(byte[] buffer, int offset, int count)
        {
            using (var hmac = new HMACSHA256(_key))
            {
                return hmac.ComputeHash(buffer, offset, count);
            }
        }

        private void VerifyMac(byte[] payload, byte[] mac)
        {
            byte[] payloadMac = ComputeMac(payload, 16 + 32, payload.Length - (16 + 32));

            for (int i = 0; i < mac.Length; i++)
            {
                if (mac[i] != payloadMac[i])
                {
                    throw new Exception("MAC mismatch");
                }
            }
        }
    }
}
 