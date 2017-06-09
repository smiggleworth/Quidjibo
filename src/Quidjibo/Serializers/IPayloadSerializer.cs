using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Commands;

namespace Quidjibo.Serializers
{
    public interface IPayloadSerializer
    {
        Task<byte[]> SerializeAsync(IQuidjiboCommand command, CancellationToken cancellationToken);
        Task<IQuidjiboCommand> DeserializeAsync(byte[] payload, CancellationToken cancellationToken);
    }
}