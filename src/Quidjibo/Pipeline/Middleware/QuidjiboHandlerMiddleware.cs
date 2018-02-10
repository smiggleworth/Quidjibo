using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quidjibo.Commands;
using Quidjibo.Models;
using Quidjibo.Pipeline.Contexts;

namespace Quidjibo.Pipeline.Middleware
{
    /// <summary>
    ///     The handler middleware dispatches the work to the correct handler.
    /// </summary>
    public class QuidjiboHandlerMiddleware : IQuidjiboMiddleware
    {
        public async Task InvokeAsync(IQuidjiboContext context, Func<Task> next, CancellationToken cancellationToken)
        {
            var logger = context.LoggerFactory.CreateLogger<QuidjiboHandlerMiddleware>();

            if(context.Command is WorkflowCommand workflow)
            {
                var tasks = workflow.Entries.Where(e => e.Key == workflow.CurrentStep)
                                    .SelectMany(e => e.Value, (e, c) => context.Dispatcher.DispatchAsync(c, context.Progress, cancellationToken))
                                    .ToList();
                await Task.WhenAll(tasks);
                workflow.NextStep();
                if(workflow.CurrentStep < workflow.Step)
                {
                    var nextPayload = await context.Serializer.SerializeAsync(workflow, cancellationToken);
                    var protectedPayload = await context.Protector.ProtectAsync(nextPayload, cancellationToken);
                    var nextItem = new WorkItem
                    {
                        Id = Guid.NewGuid(),
                        CorrelationId = context.Item.CorrelationId,
                        Attempts = 0,
                        Payload = protectedPayload,
                        Queue = context.Item.Queue
                    };
                    logger.LogDebug("Enqueue the next workflow step : {0}", nextItem.Id);
                    await context.WorkProvider.SendAsync(nextItem, 0, cancellationToken);
                }
            }
            else
            {
                await context.Dispatcher.DispatchAsync(context.Command, context.Progress, cancellationToken);
            }

            await next();
        }
    }
}