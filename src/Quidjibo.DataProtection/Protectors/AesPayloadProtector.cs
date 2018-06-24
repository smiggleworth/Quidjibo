using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.DataProtection.Constants;
using Quidjibo.DataProtection.Exceptions;
using Quidjibo.DataProtection.Providers;
using Quidjibo.Protectors;

namespace Quidjibo.DataProtection.Protectors
{
    public class AesPayloadProtector : IPayloadProtector
    {
        private readonly IKeyProvider _keyProvider;

        public AesPayloadProtector(IKeyProvider keyProvider)
        {
            _keyProvider = keyProvider;
        }

        /*
         * Encrypted payload:
         * 
         * |--- IV (16 bytes) ---|--- MAC (32 bytes) ---|--- Ciphertext (n bytes) ---|
         * 
         */
        public async Task<byte[]> ProtectAsync(byte[] payload, CancellationToken cancellationToken)
        {
            var keys = await ExpandKeysAsync(cancellationToken);
            using (var aes = Aes.Create())
            using (var crypto = aes.CreateEncryptor(keys.CipherKey, aes.IV))
            using (var stream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(stream, crypto, CryptoStreamMode.Write))
            {
                await cryptoStream.WriteAsync(payload, 0, payload.Length, cancellationToken);
                cryptoStream.FlushFinalBlock();
                var encryptedPayload = stream.ToArray();
                var mac = ComputeMac(keys.MacKey, encryptedPayload, 0, encryptedPayload.Length);
                var outputBuffer = new byte[encryptedPayload.Length + aes.IV.Length + mac.Length];
                Buffer.BlockCopy(aes.IV, 0, outputBuffer, 0, aes.IV.Length);
                Buffer.BlockCopy(mac, 0, outputBuffer, aes.IV.Length, mac.Length);
                Buffer.BlockCopy(encryptedPayload, 0, outputBuffer, aes.IV.Length + mac.Length, encryptedPayload.Length);
                return outputBuffer;
            }
        }

        public async Task<byte[]> UnprotectAsync(byte[] payload, CancellationToken cancellationToken)
        {
            var keys = await ExpandKeysAsync(cancellationToken);
            var iv = new byte[128 / 8];
            Buffer.BlockCopy(payload, 0, iv, 0, iv.Length);
            var mac = new byte[32];
            Buffer.BlockCopy(payload, iv.Length, mac, 0, 32);
            byte[] plaintext;
            using (var aes = Aes.Create())
            using (var crypto = aes.CreateDecryptor(keys.CipherKey, iv))
            using (var stream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(stream, crypto, CryptoStreamMode.Write))
            {
                await cryptoStream.WriteAsync(payload, iv.Length + mac.Length, payload.Length - iv.Length - mac.Length, cancellationToken);
                cryptoStream.FlushFinalBlock();
                plaintext = stream.ToArray();
            }

            VerifyMac(payload, mac, keys.MacKey);

            return plaintext;
        }

        private byte[] ComputeMac(byte[] macKey, byte[] buffer, int offset, int count)
        {
            using (var hmac = new HMACSHA256(macKey))
            {
                return hmac.ComputeHash(buffer, offset, count);
            }
        }

        private void VerifyMac(byte[] payload, byte[] mac, byte[] macKey)
        {
            var payloadMac = ComputeMac(macKey, payload, 16 + 32, payload.Length - (16 + 32));

            if (mac.Length != payloadMac.Length)
            {
                throw new MacMismatchException("MAC mismatch");
            }

            var mismatch = false;
            for (var i = 0; i < mac.Length; i++)
            {
                if (mac[i] != payloadMac[i])
                {
                    mismatch = true;
                }
            }

            if (mismatch)
            {
                throw new MacMismatchException("MAC mismatch");
            }
        }

        private async Task<(byte[] CipherKey, byte[] MacKey)> ExpandKeysAsync(CancellationToken cancellationToken)
        {
            var key = await _keyProvider.GetKeyAsync(cancellationToken);
            using (var hkdf = new Hkdf<HMACSHA256>())
            {
                var cipherKey = hkdf.Expand(key, KeyContext.Cipher, 32);
                var macKey = hkdf.Expand(key, KeyContext.Mac, 32);
                return (cipherKey, macKey);
            }
        }
    }
}