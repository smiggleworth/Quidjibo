using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Models;

namespace Quidjibo.Providers
{
    public interface IWorkProvider : IDisposable
    {
        /// <summary>
        ///     Sends the asynchronous.
        /// </summary>
        /// <param name="item">The work item.</param>
        /// <param name="delay">The delay in seconds.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task SendAsync(WorkItem item, int delay, CancellationToken cancellationToken);

        /// <summary>
        ///     Receives the asynchronous.
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<List<WorkItem>> ReceiveAsync(string worker, CancellationToken cancellationToken);

        /// <summary>
        ///     Renews the lock asynchronous.
        /// </summary>
        /// <param name="item">The work item.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<DateTime> RenewAsync(WorkItem item, CancellationToken cancellationToken);

        /// <summary>
        ///     Completes the asynchronous.
        /// </summary>
        /// <param name="item">The work item.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task CompleteAsync(WorkItem item, CancellationToken cancellationToken);

        /// <summary>
        ///     Faults the asynchronous.
        /// </summary>
        /// <param name="item">The work item.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task FaultAsync(WorkItem item, CancellationToken cancellationToken);
    }
}