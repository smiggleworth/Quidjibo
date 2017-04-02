using System;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Commands;
using Quidjibo.Models;

namespace Quidjibo.Handlers
{
    public interface IWorkHandler<in T> where T : IWorkCommand
    {
        Task ProcessAsync(T command, IProgress<Tracker> progress, CancellationToken cancellationToken);
    }
}