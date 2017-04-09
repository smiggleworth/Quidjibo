using System;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Handlers;
using Quidjibo.Models;

namespace Quidjibo.Autofac.Tests.Modules
{
    public class BasicHandler : IWorkHandler<BasicCommand>
    {
        public Task ProcessAsync(BasicCommand command, IProgress<Tracker> progress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}