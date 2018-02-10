using Quidjibo.DataProtection.Protectors;
using Quidjibo.DataProtection.Providers;

namespace Quidjibo.DataProtection.Extensions
{
    public static class QuidjiboBuilderExtensions
    {
        public static QuidjiboBuilder UseAes(this QuidjiboBuilder builder, byte[] aesKey)
        {
            return builder.ConfigureProtector(new AesPayloadProtector(new KeyProvider(aesKey)));
        }

        public static QuidjiboBuilder UseAes(this QuidjiboBuilder builder, IKeyProvider keyProvider)
        {
            return builder.ConfigureProtector(new AesPayloadProtector(keyProvider));
        }
    }
}