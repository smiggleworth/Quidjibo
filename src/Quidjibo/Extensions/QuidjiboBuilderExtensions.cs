using Quidjibo.Protectors;
using Quidjibo.Serializers;

namespace Quidjibo.Extensions
{
    public static class QuidjiboBuilderExtensions
    {
        public static QuidjiboBuilder UseAes(this QuidjiboBuilder builder, byte[] aesKey)
        {
            return builder.ConfigureSerializer(new PayloadSerializer(new AesPayloadProtector(aesKey)));
        }
    }
}