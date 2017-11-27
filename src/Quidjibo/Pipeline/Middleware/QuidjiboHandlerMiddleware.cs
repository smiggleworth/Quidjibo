using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quidjibo.Commands;
using Quidjibo.Dispatchers;
using Quidjibo.Models;
using Quidjibo.Pipeline.Contexts;
using Quidjibo.Protectors;
using Quidjibo.Serializers;

namespace Quidjibo.Pipeline.Middleware
{
    /// <summary>
    ///     The handler middleware dispatches the work to the correct handler.
    /// </summary>
    public class QuidjiboHandlerMiddleware : IQuidjiboMiddleware
    {
        private readonly IWorkDispatcher _dispatcher;
        private readonly ILogger _logger;
        private readonly IPayloadProtector _protector;
        private readonly IPayloadSerializer _serializer;

        public QuidjiboHandlerMiddleware(
            ILoggerFactory loggerFactory,
            IWorkDispatcher dispatcher,
            IPayloadSerializer serializer,
            IPayloadProtector protector)
        {
            _dispatcher = dispatcher;
            _serializer = serializer;
            _protector = protector;
            _logger = loggerFactory.CreateLogger<QuidjiboHandlerMiddleware>();
        }

        public async Task InvokeAsync(IQuidjiboContext context, Func<Task> next, CancellationToken cancellationToken)
        {
            var payload = await _protector.UnprotectAsync(context.Item.Payload, cancellationToken);
            var workCommand = await _serializer.DeserializeAsync(payload, cancellationToken);
            if(workCommand is WorkflowCommand workflow)
            {
                var tasks = workflow.Entries.Where(e => e.Key == workflow.CurrentStep)
                                    .SelectMany(e => e.Value, (e, c) => _dispatcher.DispatchAsync(c, context.Progress, cancellationToken))
                                    .ToList();
                await Task.WhenAll(tasks);
                workflow.NextStep();
                if(workflow.CurrentStep < workflow.Step)
                {
                    var nextPayload = await _serializer.SerializeAsync(workflow, cancellationToken);
                    var protectedPayload = await _protector.ProtectAsync(nextPayload, cancellationToken);
                    var nextItem = new WorkItem
                    {
                        Id = Guid.NewGuid(),
                        CorrelationId = context.Item.CorrelationId,
                        Attempts = 0,
                        Payload = protectedPayload,
                        Queue = context.Item.Queue
                    };
                    _logger.LogDebug("Enqueue the next workflow step : {0}", nextItem.Id);
                    await context.Provider.SendAsync(nextItem, 0, cancellationToken);
                }
            }
            else
            {
                await _dispatcher.DispatchAsync(workCommand, context.Progress, cancellationToken);
            }
            await next();
        }
    }
}