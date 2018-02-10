using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Commands;
using Quidjibo.Handlers;
using Quidjibo.Misc;

namespace Quidjibo.StructureMap.Tests.Samples
{
    public class SimpleJob
    {
        public class Command : IQuidjiboCommand
        {
            public Dictionary<string, string> Metadata { get; set; }
        }

        public class Handler : IQuidjiboHandler<Command>
        {
            public Task ProcessAsync(Command command, IQuidjiboProgress progress, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}