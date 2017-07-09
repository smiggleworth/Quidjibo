using System;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Commands;
using Quidjibo.Handlers;
using Quidjibo.Models;

namespace Quidjibo.Tests.Samples
{
    public class SimpleJob
    {
        public class Command : IQuidjiboCommand
        {
        }

        public class Handler : IQuidjiboHandler<Command>
        {
            public Task ProcessAsync(Command command, IProgress<Tracker> progress, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}