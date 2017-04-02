using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Commands;

namespace Quidjibo.Clients
{
    public interface IPublisherClient
    {
        Task PublishAsync(IWorkCommand command, CancellationToken cancellationToken = default(CancellationToken));

        Task PublishAsync(IWorkCommand command, int delay,
            CancellationToken cancellationToken = default(CancellationToken));

        Task PublishAsync(IWorkCommand command, string queueName,
            CancellationToken cancellationToken = default(CancellationToken));

        Task PublishAsync(IWorkCommand command, string queueName, int delay,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}