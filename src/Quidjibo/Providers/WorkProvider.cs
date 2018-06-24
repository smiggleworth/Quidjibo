using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Models;

namespace Quidjibo.Providers
{
    public class WorkProvider : IWorkProvider
    {
        public void Dispose()
        {
        }

        public Task SendAsync(WorkItem item, int delay, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<List<WorkItem>> ReceiveAsync(string worker, CancellationToken cancellationToken)
        {
            return Task.FromResult(new List<WorkItem>());
        }

        public Task<DateTime> RenewAsync(WorkItem item, CancellationToken cancellationToken)
        {
            return Task.FromResult(DateTime.MinValue);
        }

        public Task CompleteAsync(WorkItem item, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task FaultAsync(WorkItem item, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}