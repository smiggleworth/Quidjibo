using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Quidjibo.Azure.Storage.Configurations;
using Quidjibo.Azure.Storage.Providers;
using Quidjibo.Factories;
using Quidjibo.Providers;

namespace Quidjibo.Azure.Storage.Factories
{
    public class AzureStorageWorkProviderFactory : IWorkProviderFactory
    {
        private readonly AzureStorageQuidjiboConfiguration _configuration;

        public AzureStorageWorkProviderFactory(AzureStorageQuidjiboConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IWorkProvider> CreateAsync(string queues, CancellationToken cancellationToken = new CancellationToken())
        {
            var storageAccount = CloudStorageAccount.Parse(_configuration.ConnectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            var cloudQueue = queueClient.GetQueueReference(queues);
            var provider = new AzureStorageWorkProvider(cloudQueue, _configuration.LockInterval, _configuration.BatchSize);
            return await Task.FromResult(provider);
        }

        public Task<IWorkProvider> CreateAsync(string[] queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (queues.Length != 1)
            {
                throw new NotSupportedException("Each queues requires a seperate listener. Please pass a single queue.");
            }

            return CreateAsync(queues[0], cancellationToken);
        }
    }
}