using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Providers;

namespace Quidjibo.Factories
{
    public interface IScheduleProviderFactory
    {
        Task<IScheduleProvider> CreateAsync(string queues, CancellationToken cancellationToken = default(CancellationToken));
        Task<IScheduleProvider> CreateAsync(string[] queues, CancellationToken cancellationToken = default(CancellationToken));
    }
}