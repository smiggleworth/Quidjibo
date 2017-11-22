using System;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Commands;
using Quidjibo.Misc;
using Quidjibo.Models;

namespace Quidjibo.Clients
{
    public interface IQuidjiboClient
    {
        /// <summary>
        /// Publish a fire-and-forget
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Correlation Id</returns>
        Task<PublishInfo> PublishAsync(IQuidjiboCommand command, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Publish a fire-and-forget with a delay
        /// </summary>
        /// <param name="command"></param>
        /// <param name="delay">Delay in seconds</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Correlation Id</returns>
        Task<PublishInfo> PublishAsync(IQuidjiboCommand command, int delay, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Publish a fire-and-forget with to a specific queue
        /// </summary>
        /// <param name="command"></param>
        /// <param name="queueName">The name of the queue</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<PublishInfo> PublishAsync(IQuidjiboCommand command, string queueName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Publish a fire-and-forget with a delay to a specific queue
        /// </summary>
        /// <param name="command"></param>
        /// <param name="queueName">The name of the queue</param>
        /// <param name="delay">Delay in seconds</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<PublishInfo> PublishAsync(IQuidjiboCommand command, string queueName, int delay, CancellationToken cancellationToken = default(CancellationToken));

        Task ScheduleAsync(string name, IQuidjiboCommand command, Cron cron
            , CancellationToken cancellationToken = default(CancellationToken));

        Task ScheduleAsync(string name, string queue, IQuidjiboCommand command, Cron cron,
            CancellationToken cancellationToken = default(CancellationToken));

        void Clear();
    }
}