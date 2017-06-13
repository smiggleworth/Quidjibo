using System;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Factories;
using Quidjibo.Providers;

namespace Quidjibo.Azure.Storage.Factories
{
    public class AzureStorageWorkProviderFactory : IWorkProviderFactory
    {
        public Task<IWorkProvider> CreateAsync(string queue, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }
    }
}