using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Providers;

namespace Quidjibo.Factories
{
    public class ScheduleProviderFactory : IScheduleProviderFactory
    {
        public Task<IScheduleProvider> CreateAsync(string queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<IScheduleProvider>(new ScheduleProvider());
        }

        public Task<IScheduleProvider> CreateAsync(string[] queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<IScheduleProvider>(new ScheduleProvider());
        }
    }
}