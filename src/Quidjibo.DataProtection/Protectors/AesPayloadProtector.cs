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
        private const int VERSION_SIZE = 1;
        private const int IV_SIZE = 16;
        private const int MAC_SIZE = 32;
        private static readonly byte[] _version1 = new byte[] { 1 };

        private readonly IKeyProvider _keyProvider;

        public AesPayloadProtector(IKeyProvider keyProvider)
        {
            _keyProvider = keyProvider;
        }

        /*
         * Encrypted payload:
         * 
         * |--- Version (1 byte) ---|--- IV (16 bytes) ---|--- MAC (32 bytes) ---|--- Ciphertext (n bytes) ---|
         * 
         */
        public async Task<byte[]> ProtectAsync(byte[] payload, CancellationToken cancellationToken)
        {
            var version = new ArraySegment<byte>(_version1);
            var keys = await ExpandKeysAsync(cancellationToken);
            using (var aes = Aes.Create())
            using (var crypto = aes.CreateEncryptor(keys.CipherKey, aes.IV))
            using (var stream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(stream, crypto, CryptoStreamMode.Write))
            {
                await cryptoStream.WriteAsync(payload, 0, payload.Length, cancellationToken);
                cryptoStream.FlushFinalBlock();
                var encryptedPayload = stream.ToArray();
                var ivSegment = new ArraySegment<byte>(aes.IV);
                var payloadSegment = new ArraySegment<byte>(encryptedPayload);
                var mac = ComputeMac(keys.MacKey, version, ivSegment, payloadSegment);
                var outputBuffer = new byte[VERSION_SIZE + IV_SIZE + MAC_SIZE + encryptedPayload.Length];
                Memory<byte> outputBufferMemory = outputBuffer;
                _version1.CopyTo(outputBufferMemory);
                aes.IV.CopyTo(outputBufferMemory.Slice(VERSION_SIZE));
                mac.CopyTo(outputBufferMemory.Slice(VERSION_SIZE + IV_SIZE));
                encryptedPayload.CopyTo(outputBufferMemory.Slice(VERSION_SIZE + IV_SIZE + MAC_SIZE));
                return outputBuffer;
            }
        }

        public async Task<byte[]> UnprotectAsync(byte[] payload, CancellationToken cancellationToken)
        {
            if (!payload.AsSpan().Slice(0, 1).SequenceEqual(_version1))
            {
                throw new InvalidOperationException("Unknown payload version.");
            }
            var keys = await ExpandKeysAsync(cancellationToken);
            var iv = new byte[IV_SIZE];
            Buffer.BlockCopy(payload, VERSION_SIZE, iv, 0, iv.Length);
            var mac = new byte[MAC_SIZE];
            Buffer.BlockCopy(payload, VERSION_SIZE + IV_SIZE, mac, 0, mac.Length);
            VerifyMac(payload, mac, keys.MacKey);
            byte[] plaintext;
            using (var aes = Aes.Create())
            using (var crypto = aes.CreateDecryptor(keys.CipherKey, iv))
            using (var stream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(stream, crypto, CryptoStreamMode.Write))
            {
                var payloadPosition = VERSION_SIZE + MAC_SIZE + IV_SIZE;
                await cryptoStream.WriteAsync(payload, payloadPosition, payload.Length - payloadPosition, cancellationToken);
                cryptoStream.FlushFinalBlock();
                plaintext = stream.ToArray();
            }

            return plaintext;
        }

        private byte[] ComputeMac(byte[] macKey, ArraySegment<byte> version, ArraySegment<byte> iv, ArraySegment<byte> ciphertext)
        {
            using (var hmac = IncrementalHash.CreateHMAC(HashAlgorithmName.SHA256, macKey))
            {
                hmac.AppendData(version.Array, version.Offset, version.Count);
                hmac.AppendData(iv.Array, iv.Offset, iv.Count);
                hmac.AppendData(ciphertext.Array, ciphertext.Offset, ciphertext.Count);
                return hmac.GetHashAndReset();
            }
        }

        private void VerifyMac(byte[] payload, byte[] mac, byte[] macKey)
        {
            if (payload.Length < VERSION_SIZE + IV_SIZE + MAC_SIZE)
            {
                throw new MacMismatchException("MAC mismatch");
            }
            var versionSegment = new ArraySegment<byte>(payload, 0, VERSION_SIZE);
            var ivSegment = new ArraySegment<byte>(payload, VERSION_SIZE, IV_SIZE);
            var macSegment = new ArraySegment<byte>(payload, VERSION_SIZE + IV_SIZE, MAC_SIZE);
            var payloadSegment = new ArraySegment<byte>(
                payload,
                VERSION_SIZE + IV_SIZE + MAC_SIZE,
                payload.Length - (VERSION_SIZE + IV_SIZE + MAC_SIZE));
            var payloadMac = ComputeMac(macKey, versionSegment, ivSegment, payloadSegment);

            if (mac.Length != payloadMac.Length)
            {
                throw new MacMismatchException("MAC mismatch");
            }

            var mismatch = 0;
            for (var i = 0; i < mac.Length; i++)
            {
                mismatch |= mac[i] ^ payloadMac[i];
            }

            if (mismatch != 0)
            {
                throw new MacMismatchException("MAC mismatch");
            }
        }

        private async Task<(byte[] CipherKey, byte[] MacKey)> ExpandKeysAsync(CancellationToken cancellationToken)
        {
            var key = await _keyProvider.GetKeyAsync(cancellationToken);
            var hkdf = new Hkdf(HashAlgorithmName.SHA256);
            var cipherKey = hkdf.Expand(key, KeyContext.Cipher, 32);
            var macKey = hkdf.Expand(key, KeyContext.Mac, 32);
            return (cipherKey, macKey);
        }
    }
}