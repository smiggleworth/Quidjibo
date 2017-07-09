using System;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Commands;
using Quidjibo.Misc;
using Quidjibo.Models;

namespace Quidjibo.Clients
{
    public interface IQuidjiboClient : IQuidjiboClient<DefaultClientKey>
    {
    }

    public interface IQuidjiboClient<TKey> : IDisposable
        where TKey : IQuidjiboClientKey
    {
        Task PublishAsync(IQuidjiboCommand command,
            CancellationToken cancellationToken = default(CancellationToken));

        Task PublishAsync(IQuidjiboCommand command, int delay,
            CancellationToken cancellationToken = default(CancellationToken));

        Task PublishAsync(IQuidjiboCommand command, string queueName,
            CancellationToken cancellationToken = default(CancellationToken));

        Task PublishAsync(IQuidjiboCommand command, string queueName, int delay,
            CancellationToken cancellationToken = default(CancellationToken));

        Task ScheduleAsync(string name, IQuidjiboCommand command, Cron cron
            , CancellationToken cancellationToken = default(CancellationToken));

        Task ScheduleAsync(string name, string queue, IQuidjiboCommand command, Cron cron,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}