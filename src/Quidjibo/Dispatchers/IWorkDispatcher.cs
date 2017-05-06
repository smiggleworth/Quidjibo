using System;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Commands;
using Quidjibo.Models;

namespace Quidjibo.Dispatchers
{
    public interface IWorkDispatcher
    {
        Task DispatchAsync(IQuidjiboCommand command, IProgress<Tracker> progress, CancellationToken cancellationToken);
    }
}