using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Commands;
using Quidjibo.Models;

namespace Quidjibo.Clients
{
    public interface IQuidjiboClient
    {
        /// <summary>
        ///     Publish a fire-and-forget
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Correlation Id</returns>
        Task<PublishInfo> PublishAsync(IQuidjiboCommand command, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Publish a fire-and-forget with a delay
        /// </summary>
        /// <param name="command"></param>
        /// <param name="delay">Delay in seconds</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Correlation Id</returns>
        Task<PublishInfo> PublishAsync(IQuidjiboCommand command, int delay, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Publish a fire-and-forget with to a specific queue
        /// </summary>
        /// <param name="command"></param>
        /// <param name="queueName">The name of the queue</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<PublishInfo> PublishAsync(IQuidjiboCommand command, string queueName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Publish a fire-and-forget with a delay to a specific queue
        /// </summary>
        /// <param name="command"></param>
        /// <param name="queueName">The name of the queue</param>
        /// <param name="delay">Delay in seconds</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<PublishInfo> PublishAsync(IQuidjiboCommand command, string queueName, int delay, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Schedule a recurring job with a specified schedule
        /// </summary>
        /// <param name="name">The name of the job</param>
        /// <param name="command">The recurring command</param>
        /// <param name="cron">The cron schedule</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        Task ScheduleAsync(string name, IQuidjiboCommand command, Cron cron, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Schedule a recurring job with a specified schedule
        /// </summary>
        /// <param name="name">The name of the job</param>
        /// <param name="queue">The queue that should be used</param>
        /// <param name="command">The recurring command</param>
        /// <param name="cron">The cron schedule</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        Task ScheduleAsync(string name, string queue, IQuidjiboCommand command, Cron cron, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Delete a previously scheduled recurring job
        /// </summary>
        /// <param name="name">The name of the job to delete</param>
        /// <param name="queue">The queue the job belongs to</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        Task DeleteScheduleAsync(string name, string queue, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Clear internally static client instance
        /// </summary>
        void Clear();
    }
}