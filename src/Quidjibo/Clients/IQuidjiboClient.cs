using System;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Commands;
using Quidjibo.Models;

namespace Quidjibo.Clients
{
    public interface IQuidjiboClient : IDisposable
    {
        Task PublishAsync(IWorkCommand command,
            CancellationToken cancellationToken = default(CancellationToken));

        Task PublishAsync(IWorkCommand command, int delay,
            CancellationToken cancellationToken = default(CancellationToken));

        Task PublishAsync(IWorkCommand command, string queueName,
            CancellationToken cancellationToken = default(CancellationToken));

        Task PublishAsync(IWorkCommand command, string queueName, int delay,
            CancellationToken cancellationToken = default(CancellationToken));

        Task ScheduleAsync(string name, IWorkCommand command, Cron cron
            , CancellationToken cancellationToken = default(CancellationToken));

        Task ScheduleAsync(string name, string queue, IWorkCommand command, Cron cron,
            CancellationToken cancellationToken = new CancellationToken());
    }
}