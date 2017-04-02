using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Commands;
using Quidjibo.Extensions;
using Quidjibo.Factories;
using Quidjibo.Models;
using Quidjibo.Protectors;
using Quidjibo.Providers;
using Quidjibo.Serializers;

namespace Quidjibo.Clients
{
    public class PublisherClient : IPublisherClient
    {
        private static readonly ConcurrentDictionary<string, IWorkProvider> Providers =
            new ConcurrentDictionary<string, IWorkProvider>();

        private readonly IPayloadSerializer _payloadSerializer;

        private readonly IWorkProviderFactory _workProviderFactory;

        public PublisherClient(IWorkProviderFactory workProviderFactory)
            : this(workProviderFactory, new PayloadSerializer(new PayloadProtector()))
        {
        }

        public PublisherClient(
            IWorkProviderFactory workProviderFactory,
            IPayloadSerializer payloadSerializer)
        {
            _workProviderFactory = workProviderFactory;
            _payloadSerializer = payloadSerializer;
        }

        public async Task PublishAsync(IWorkCommand command,
            CancellationToken cancellationToken = new CancellationToken())
        {
            await PublishAsync(command, 0, cancellationToken);
        }

        public async Task PublishAsync(IWorkCommand command, int delay,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var queueName = command.GetQueueName();
            await PublishAsync(command, queueName, delay, cancellationToken);
        }

        public async Task PublishAsync(IWorkCommand command, string queueName,
            CancellationToken cancellationToken = new CancellationToken())
        {
            await PublishAsync(command, queueName, 0, cancellationToken);
        }

        public async Task PublishAsync(IWorkCommand command, string queueName, int delay,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var item = new WorkItem
            {
                Id = Guid.NewGuid(),
                CorrelationId = Guid.NewGuid(),
                Name = command.GetName(),
                Attempts = 0,
                Payload = _payloadSerializer.Serialize(command),
                Queue = queueName
            };
            var provider = await GetOrCreateWorkProvider(queueName, cancellationToken);

            await provider.SendAsync(item, 0, cancellationToken);
        }

        private async Task<IWorkProvider> GetOrCreateWorkProvider(string queueName, CancellationToken cancellationToken)
        {
            IWorkProvider provider;
            if (!Providers.TryGetValue(queueName, out provider))
            {
                provider = await _workProviderFactory.CreateAsync(queueName, cancellationToken);
                Providers.TryAdd(queueName, provider);
            }
            return provider;
        }
    }
}