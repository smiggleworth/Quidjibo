using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Models;
using Quidjibo.Providers;

namespace Quidjibo.Azure.Storage.Providers
{
    public class AzureStorageWorkProvider : IWorkProvider
    {
        public Task SendAsync(WorkItem item, int delay, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<WorkItem>> ReceiveAsync(string worker, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<DateTime> RenewAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task CompleteAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task FaultAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}