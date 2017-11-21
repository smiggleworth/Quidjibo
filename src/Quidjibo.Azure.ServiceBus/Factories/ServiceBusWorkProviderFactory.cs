﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Quidjibo.Azure.ServiceBus.Providers;
using Quidjibo.Factories;
using Quidjibo.Providers;

namespace Quidjibo.Azure.ServiceBus.Factories
{
    public class ServiceBusWorkProviderFactory : IWorkProviderFactory
    {
        private readonly int _batchSize;
        private readonly string _connectionString;
        private readonly int _prefectchCount;
        private readonly RetryPolicy _retryPolicy;

        public ServiceBusWorkProviderFactory(string connectionString, RetryPolicy retryPolicy, int prefectchCount, int batchSize)
        {
            _connectionString = connectionString;
            _retryPolicy = retryPolicy;
            _prefectchCount = prefectchCount;
            _batchSize = batchSize;
        }

        public int PollingInterval => 10;

        public async Task<IWorkProvider> CreateAsync(string queues, CancellationToken cancellationToken = new CancellationToken())
        {
            var sender = new MessageSender(_connectionString, queues, RetryPolicy.Default);
            var receiver = new MessageReceiver(_connectionString, queues, ReceiveMode.PeekLock, _retryPolicy, _prefectchCount);


            var provider = new ServiceBusWorkProvider(sender, receiver, _batchSize);

            // todo : Create queues if not exists


            return await Task.FromResult(provider);
        }

        public Task<IWorkProvider> CreateAsync(string[] queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new System.NotImplementedException();
        }
    }
}