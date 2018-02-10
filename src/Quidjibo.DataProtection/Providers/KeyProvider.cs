using System.Threading;
using System.Threading.Tasks;

namespace Quidjibo.DataProtection.Providers
{
    public class KeyProvider : IKeyProvider
    {
        private readonly byte[] _key;

        public KeyProvider(byte[] key)
        {
            _key = key;
        }

        public Task<byte[]> GetKeyAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_key);
        }
    }
}