using System;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Commands;
using Quidjibo.Handlers;
using Quidjibo.Models;

namespace Quidjibo.Autofac.Tests.Modules
{
    public class SimpleJob
    {

        public class Command : IWorkCommand
        {
        }

        public class Handler : IWorkHandler<Command>
        {
            public Task ProcessAsync(Command command, IProgress<Tracker> progress, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}