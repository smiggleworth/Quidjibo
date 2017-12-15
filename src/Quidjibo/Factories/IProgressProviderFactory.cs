using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Providers;

namespace Quidjibo.Factories
{
    public interface IProgressProviderFactory
    {
        Task<IProgressProvider> CreateAsync(string queues, CancellationToken cancellationToken = default(CancellationToken));
        Task<IProgressProvider> CreateAsync(string[] queues, CancellationToken cancellationToken = default(CancellationToken));
    }
}