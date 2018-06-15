using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Commands;
using Quidjibo.EndToEnd.Services;
using Quidjibo.Handlers;
using Quidjibo.Misc;

namespace Quidjibo.EndToEnd.Jobs
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

            public Guid? CorrelationId { get; set; }
            public Dictionary<string, string> Metadata { get; set; }
        }

        public class Handler : IQuidjiboHandler<Command>
        {
            private readonly ISimpleService _simpleService;

            public Handler(ISimpleService simpleService)
            {
                _simpleService = simpleService;
            }

            public async Task ProcessAsync(Command command, IQuidjiboProgress progress, CancellationToken cancellationToken)
            {
                progress.Report(1, $"Starting item {command.Id}");
                await _simpleService.DoWorkAsync(cancellationToken);
                progress.Report(100, $"Finished item {command.Id}");
            }
        }
    }
}