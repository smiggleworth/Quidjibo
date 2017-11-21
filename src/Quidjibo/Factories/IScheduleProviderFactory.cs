using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Providers;

namespace Quidjibo.Factories
{
    public interface IScheduleProviderFactory
    {
        /// <summary>
        ///     The polling interval in seconds
        /// </summary>
        int PollingInterval { get; }

        Task<IScheduleProvider> CreateAsync(string queues, CancellationToken cancellationToken = default(CancellationToken));
        Task<IScheduleProvider> CreateAsync(string[] queues, CancellationToken cancellationToken = default(CancellationToken));
    }
}