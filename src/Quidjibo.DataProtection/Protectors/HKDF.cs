using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Quidjibo.DataProtection.Protectors
{
    public class Hkdf
    {
        private readonly HashAlgorithmName _hashAlgorithmName;

        public Hkdf(HashAlgorithmName hashAlgorithmName)
        {
            _hashAlgorithmName = hashAlgorithmName;
        }

        /// <summary>
        /// </summary>
        /// <param name="salt"></param>
        /// <param name="inputKeyMaterial"></param>
        /// <returns>A pseudorandom key.</returns>
        public byte[] Extract(byte[] salt, byte[] inputKeyMaterial)
        {
            var hmac = IncrementalHash.CreateHMAC(_hashAlgorithmName, salt ?? Array.Empty<byte>());
            hmac.AppendData(inputKeyMaterial);
            return hmac.GetHashAndReset();
        }

        /// <summary>
        /// </summary>
        /// <param name="pseudoRandomKey"></param>
        /// <param name="info"></param>
        /// <param name="length"></param>
        /// <returns>Output keying material</returns>
        public byte[] Expand(byte[] pseudoRandomKey, byte[] info, int length)
        {
            var hashSizeBytes = HashAlgorithmSize(_hashAlgorithmName);
            if (length > hashSizeBytes * 255)
            {
                throw new Exception("Invalid length. Must be less than or equal to 255 * Hash Length.");
            }

            info = info ?? Array.Empty<byte>();

            using (var hmac = IncrementalHash.CreateHMAC(_hashAlgorithmName, pseudoRandomKey))
            {
                var n = (length + (hashSizeBytes - 1)) / hashSizeBytes;

                var t = new byte[length];
                Span<byte> target = t;
                // We can write all but the last N T's directly to the Span. We handle the
                // last T outside of the loop since the detination may be smaller than the MAC.
                var index = new byte[] { 1 } ;
                Span<byte> indexSpan = index;
                int previous = -hashSizeBytes;
                for (ref byte i = ref indexSpan[0]; i < n; i++)
                {
                    if (previous >= 0)
                    {
                        hmac.AppendData(t, previous, hashSizeBytes);
                    }
                    hmac.AppendData(info);
                    hmac.AppendData(index);
                    Span<byte> mac = hmac.GetHashAndReset();
                    mac.CopyTo(target);
                    target = target.Slice(hashSizeBytes);
                    previous += hashSizeBytes;
                }
                if (previous >= 0)
                {
                    hmac.AppendData(t, previous, hashSizeBytes);
                }
                hmac.AppendData(info);
                hmac.AppendData(index);
                Span<byte> finalMac = hmac.GetHashAndReset();
                finalMac.Slice(0, target.Length).CopyTo(target);
                return t;
            }
        }

        private static int HashAlgorithmSize(HashAlgorithmName hashAlgorithmName)
        {
            if (hashAlgorithmName == HashAlgorithmName.MD5)
            {
                return 128 / 8;
            }
            if (hashAlgorithmName == HashAlgorithmName.SHA1)
            {
                return 160 / 8;
            }
            if (hashAlgorithmName == HashAlgorithmName.SHA256)
            {
                return 256 / 8;
            }
            if (hashAlgorithmName == HashAlgorithmName.SHA384)
            {
                return 384 / 8;
            }
            if (hashAlgorithmName == HashAlgorithmName.SHA512)
            {
                return 512 / 8;
            }
            throw new ArgumentException("Unknown hash algorithm.", nameof(hashAlgorithmName));
        }
    }
}