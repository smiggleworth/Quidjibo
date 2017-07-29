using System;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Commands;
using Quidjibo.Handlers;
using Quidjibo.Models;

namespace Quidjibo.EndToEnd
{
    public class Job
    {
        public class Command : IQuidjiboCommand
        {
            public int Id { get; }

            public Command(int id)
            {
                Id = id;
            }
        }

        public class Handler : IQuidjiboHandler<Command>
        {
            public async Task ProcessAsync(Command command, IProgress<Tracker> progress, CancellationToken cancellationToken)
            {
                progress.Report(new Tracker(0, $"Starting : {command.Id}"));

                await Task.Delay(10, cancellationToken);
                progress.Report(new Tracker(50, $"Half way : {command.Id}"));

                await Task.Delay(10, cancellationToken);
                progress.Report(new Tracker(100, $"Finished : {command.Id}"));
            }
        }
    }
}