using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Providers;

namespace Quidjibo.Factories
{
    public class
        WorkProviderFactory : IWorkProviderFactory
    {
        public Task<IWorkProvider> CreateAsync(string queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<IWorkProvider>(new WorkProvider());
        }

        public Task<IWorkProvider> CreateAsync(string[] queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<IWorkProvider>(new WorkProvider());
        }
    }
}