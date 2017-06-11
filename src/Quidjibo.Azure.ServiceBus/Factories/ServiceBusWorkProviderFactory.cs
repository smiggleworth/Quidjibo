using System;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Factories;
using Quidjibo.Providers;

namespace Quidjibo.Azure.ServiceBus.Factories
{
    public class ServiceBusWorkProviderFactory : IWorkProviderFactory
    {
        public Task<IWorkProvider> CreateAsync(string queue, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }
    }
}