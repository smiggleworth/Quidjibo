﻿using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quidjibo.Attributes;
using Quidjibo.Commands;
using Quidjibo.Constants;
using Quidjibo.Extensions;
using Quidjibo.Factories;
using Quidjibo.Models;
using Quidjibo.Protectors;
using Quidjibo.Providers;
using Quidjibo.Serializers;

namespace Quidjibo.Clients
{

    public class QuidjiboClient : IQuidjiboClient
    {
        private readonly ICronProvider _cronProvider;
        private readonly ILogger _logger;
        private readonly IPayloadProtector _payloadProtector;
        private readonly IPayloadSerializer _payloadSerializer;
        private readonly IScheduleProviderFactory _scheduleProviderFactory;
        private readonly IWorkProviderFactory _workProviderFactory;

        private bool _disposed;
        public static IQuidjiboClient Instance { get; set; }

        public QuidjiboClient(
            ILoggerFactory loggerFactory,
            IWorkProviderFactory workProviderFactory,
            IScheduleProviderFactory scheduleProviderFactory,
            IPayloadSerializer payloadSerializer,
            IPayloadProtector payloadProtector,
            ICronProvider cronProvider)
        {
            _logger = loggerFactory.CreateLogger(GetType());
            _workProviderFactory = workProviderFactory;
            _scheduleProviderFactory = scheduleProviderFactory;
            _payloadSerializer = payloadSerializer;
            _payloadProtector = payloadProtector;
            _cronProvider = cronProvider;
        }

        public Task<PublishInfo> PublishAsync(IQuidjiboCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            return PublishAsync(command, 0, cancellationToken);
        }

        public Task<PublishInfo> PublishAsync(IQuidjiboCommand command, int delay, CancellationToken cancellationToken = default(CancellationToken))
        {
            var queueName = command.GetQueueName();
            return PublishAsync(command, queueName, delay, cancellationToken);
        }

        public Task<PublishInfo> PublishAsync(IQuidjiboCommand command, string queueName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return PublishAsync(command, queueName, 0, cancellationToken);
        }

        public async Task<PublishInfo> PublishAsync(IQuidjiboCommand command, string queueName, int delay, CancellationToken cancellationToken = default(CancellationToken))
        {
            var options = new PublishOptions
            {
                Delay = delay,
                ExpireOn = default(DateTime)
            };
            return await PublishAsync(command, queueName, options, cancellationToken);
        }

        public async Task<PublishInfo> PublishAsync(IQuidjiboCommand command, string queueName, PublishOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            var payload = await _payloadSerializer.SerializeAsync(command, cancellationToken);
            var protectedPayload = await _payloadProtector.ProtectAsync(payload, cancellationToken);
            var item = new WorkItem
            {
                Id = Guid.NewGuid(),
                CorrelationId = command.CorrelationId ?? Guid.NewGuid(),
                Name = command.GetName(),
                Attempts = 0,
                Payload = protectedPayload,
                Queue = queueName,
                ExpireOn = options.ExpireOn
            };
            var provider = await GetOrCreateWorkProvider(queueName, cancellationToken);
            await provider.SendAsync(item, options.Delay, cancellationToken);
            return new PublishInfo(item.Id, item.CorrelationId);
        }

        public async Task ScheduleAsync(string name, IQuidjiboCommand command, Cron cron, CancellationToken cancellationToken = default(CancellationToken))
        {
            var queueName = command.GetQueueName();
            await ScheduleAsync(name, queueName, command, cron, cancellationToken);
        }

        public async Task ScheduleAsync(string name, string queue, IQuidjiboCommand command, Cron cron, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogWarning("The name argument is required");
                return;
            }

            if (string.IsNullOrWhiteSpace(queue))
            {
                _logger.LogWarning("The queue argument is required");
                return;
            }

            if (command == null)
            {
                _logger.LogWarning("The command argument is required");
                return;
            }

            if (cron == null)
            {
                _logger.LogWarning("The cron argument is required");
                return;
            }

            var now = DateTime.UtcNow;
            var payload = await _payloadSerializer.SerializeAsync(command, cancellationToken);
            var protectedPayload = await _payloadProtector.ProtectAsync(payload, cancellationToken);
            var item = new ScheduleItem
            {
                CreatedOn = now,
                CronExpression = cron.Expression,
                EnqueueOn = _cronProvider.GetNextSchedule(cron.Expression),
                Id = Guid.NewGuid(),
                Name = name,
                Payload = protectedPayload,
                Queue = queue,
                VisibleOn = now
            };
            var provider = await GetOrCreateScheduleProvider(queue, cancellationToken);
            var existingItem = await provider.LoadByNameAsync(name, cancellationToken);
            if (existingItem != null)
            {
                if (!item.EquivalentTo(existingItem))
                {
                    _logger.LogDebug("Replace existing schedule for {Name}", name);
                    await provider.DeleteAsync(existingItem.Id, cancellationToken);
                    await provider.CreateAsync(item, cancellationToken);
                }
            }
            else
            {
                _logger.LogDebug("Creating new schedule for {Name}", name);
                await provider.CreateAsync(item, cancellationToken);
            }
        }

        public async Task DeleteScheduleAsync(string name, string queue, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogWarning("The name argument is required");
                return;
            }

            if (string.IsNullOrWhiteSpace(queue))
            {
                _logger.LogWarning("The queue argument is required");
                return;
            }

            var provider = await GetOrCreateScheduleProvider(queue, cancellationToken);
            var existingItem = await provider.LoadByNameAsync(name, cancellationToken);
            if (existingItem != null)
            {
                _logger.LogDebug("Delete existing schedule for {Name}", name);
                await provider.DeleteAsync(existingItem.Id, cancellationToken);
            }
        }

        public void Clear()
        {
            Instance = null;
        }

        public async Task ScheduleAsync(Assembly[] assemblies, CancellationToken cancellationToken)
        {
            if (assemblies == null)
            {
                return;
            }

            var schedules = from a in assemblies
                            from t in a.GetExportedTypes()
                            where typeof(IQuidjiboCommand).IsAssignableFrom(t)
                            from attr in t.GetTypeInfo().GetCustomAttributes<ScheduleAttribute>()
                            let name = attr.Name
                            let queue = !string.IsNullOrWhiteSpace(attr.Queue) ? attr.Queue : Default.Queue
                            let command = (IQuidjiboCommand)Activator.CreateInstance(t)
                            let cron = attr.Cron
                            select ScheduleAsync(name, queue, command, cron, cancellationToken);

            await Task.WhenAll(schedules);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Clear();
            _disposed = true;
        }

        private Task<IWorkProvider> GetOrCreateWorkProvider(string queueName, CancellationToken cancellationToken)
        {
            return _workProviderFactory.CreateAsync(queueName, cancellationToken);
        }

        private Task<IScheduleProvider> GetOrCreateScheduleProvider(string queueName, CancellationToken cancellationToken)
        {
            return _scheduleProviderFactory.CreateAsync(queueName, cancellationToken);
        }
    }
}