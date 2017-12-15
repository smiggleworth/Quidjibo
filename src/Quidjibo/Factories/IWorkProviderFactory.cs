using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Providers;

namespace Quidjibo.Factories
{
    public interface IWorkProviderFactory
    {
        /// <summary>
        ///     The polling interval in seconds
        /// </summary>
        int PollingInterval { get; }

        Task<IWorkProvider> CreateAsync(string queues, CancellationToken cancellationToken = default(CancellationToken));
        Task<IWorkProvider> CreateAsync(string[] queues, CancellationToken cancellationToken = default(CancellationToken));
    }
}