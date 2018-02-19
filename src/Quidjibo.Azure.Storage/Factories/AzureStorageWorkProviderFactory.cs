using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Quidjibo.Azure.Storage.Providers;
using Quidjibo.Factories;
using Quidjibo.Providers;

namespace Quidjibo.Azure.Storage.Factories
{
    public class AzureStorageWorkProviderFactory : IWorkProviderFactory
    {
        private readonly int _batchSize;
        private readonly string _connectionString;
        private readonly int _visibilityTimeout;

        public AzureStorageWorkProviderFactory(string connectionString, int visibilityTimeout = 60, int batchSize = 5)
        {
            _connectionString = connectionString;
            _visibilityTimeout = visibilityTimeout;
            _batchSize = batchSize;
        }


        public int PollingInterval => 10;

        public async Task<IWorkProvider> CreateAsync(string queues, CancellationToken cancellationToken = new CancellationToken())
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            var cloudQueue = queueClient.GetQueueReference(queues);
            var provider = new AzureStorageWorkProvider(cloudQueue, _visibilityTimeout, _batchSize);
            return await Task.FromResult(provider);
        }

        public Task<IWorkProvider> CreateAsync(string[] queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}