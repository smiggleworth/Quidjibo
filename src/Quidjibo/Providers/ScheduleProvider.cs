using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Models;

namespace Quidjibo.Providers
{
    public class ScheduleProvider : IScheduleProvider
    {
        public Task<List<ScheduleItem>> ReceiveAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(new List<ScheduleItem>());
        }

        public Task CompleteAsync(ScheduleItem item, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public Task CreateAsync(ScheduleItem item, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public Task<ScheduleItem> LoadByNameAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(default(ScheduleItem));
        }

        public Task UpdateAsync(ScheduleItem item, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(false);
        }
    }
}