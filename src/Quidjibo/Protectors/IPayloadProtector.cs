using System.Threading;
using System.Threading.Tasks;

namespace Quidjibo.Protectors
{
    public interface IPayloadProtector
    {
        Task<byte[]> ProtectAsync(byte[] payload, CancellationToken cancellationToken);
        Task<byte[]> UnprotectAysnc(byte[] payload, CancellationToken cancellationToken);
    }
}