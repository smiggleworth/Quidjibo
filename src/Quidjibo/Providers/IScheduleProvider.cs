using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Models;

namespace Quidjibo.Providers
{
    public interface IScheduleProvider
    {
        Task<List<ScheduleItem>> ReceiveAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task CompleteAsync(ScheduleItem item, CancellationToken cancellationToken = default(CancellationToken));
        Task CreateAsync(ScheduleItem item, CancellationToken cancellationToken = default(CancellationToken));
        Task<ScheduleItem> LoadByNameAsync(string name, CancellationToken cancellationToken = default(CancellationToken));
        Task UpdateAsync(ScheduleItem item, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default(CancellationToken));
    }
}