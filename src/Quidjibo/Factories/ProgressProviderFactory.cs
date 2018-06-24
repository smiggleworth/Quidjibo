using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Providers;

namespace Quidjibo.Factories
{
    public class ProgressProviderFactory : IProgressProviderFactory
    {
        public Task<IProgressProvider> CreateAsync(string queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<IProgressProvider>(new ProgressProvider());
        }

        public Task<IProgressProvider> CreateAsync(string[] queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<IProgressProvider>(new ProgressProvider());
        }
    }
}