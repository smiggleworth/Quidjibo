using System.Threading;
using System.Threading.Tasks;

namespace Quidjibo.Protectors
{
    public interface IPayloadProtector
    {
        Task<byte[]> ProtectAsync(byte[] payload, CancellationToken cancellationToken);
        Task<byte[]> UnprotectAsync(byte[] payload, CancellationToken cancellationToken);
    }
}