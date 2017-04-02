using System;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Attributes;
using Quidjibo.Commands;
using Quidjibo.Handlers;
using Quidjibo.Models;

namespace Quidjibo.Sample.Jobs
{
    [QueueName("other-stuff")]
    public class LongRunningJob
    {
        public class Command : IWorkCommand
        {
            public string Hello { get; }

            public Command(string hello)
            {
                Hello = hello;
            }
        }

        public class Handler : IWorkHandler<Command>
        {
            public async Task ProcessAsync(Command command, IProgress<Tracker> progress,
                CancellationToken cancellationToken)
            {
                await Task.Delay(TimeSpan.FromSeconds(45), cancellationToken);
                Console.WriteLine($"Processing {command.Hello}");
            }
        }
    }
}