using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Providers;

namespace Quidjibo.Factories
{
    public interface IWorkProviderFactory
    {
        Task<IWorkProvider> CreateAsync(string queues, CancellationToken cancellationToken = default(CancellationToken));
        Task<IWorkProvider> CreateAsync(string[] queues, CancellationToken cancellationToken = default(CancellationToken));
    }
}