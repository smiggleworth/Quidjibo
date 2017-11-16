using System.Threading;
using System.Threading.Tasks;

namespace Quidjibo.Protectors
{
    public class PayloadProtector : IPayloadProtector
    {
        public Task<byte[]> ProtectAsync(byte[] payload, CancellationToken cancellationToken)
        {
            return Task.FromResult(payload);
        }

        public Task<byte[]> UnprotectAsync(byte[] payload, CancellationToken cancellationToken)
        {
            return Task.FromResult(payload);
        }
    }
}