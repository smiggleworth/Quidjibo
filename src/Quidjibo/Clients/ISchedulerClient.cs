using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Commands;
using Quidjibo.Models;

namespace Quidjibo.Clients
{
    public interface ISchedulerClient
    {
        Task ScheduleAsync(string name, IWorkCommand command, Cron cron, CancellationToken cancellationToken = default(CancellationToken));
        Task ScheduleAsync(string name, string queue, IWorkCommand command, Cron cron, CancellationToken cancellationToken = new CancellationToken());
    }
}