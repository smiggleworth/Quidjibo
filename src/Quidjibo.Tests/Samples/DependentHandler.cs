using System;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Handlers;
using Quidjibo.Models;

namespace Quidjibo.Tests.Samples
{
    public class DependentHandler : IQuidjiboHandler<DependentCommand>
    {
        public DependentHandler(IDependency dependency)
        {
        }

        public async Task ProcessAsync(DependentCommand command, IProgress<Tracker> progress, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}