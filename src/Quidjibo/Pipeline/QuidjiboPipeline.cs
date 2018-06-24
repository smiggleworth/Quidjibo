using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quidjibo.Dispatchers;
using Quidjibo.Pipeline.Contexts;
using Quidjibo.Pipeline.Middleware;
using Quidjibo.Pipeline.Misc;
using Quidjibo.Protectors;
using Quidjibo.Resolvers;
using Quidjibo.Serializers;

namespace Quidjibo.Pipeline
{
    public class QuidjiboPipeline : IQuidjiboPipeline
    {
        private readonly IWorkDispatcher _dispatcher;

        private readonly ILogger _logger;

        private readonly ILoggerFactory _loggerFactory;

        private readonly IPayloadProtector _protector;

        private readonly IDependencyResolver _resolver;

        private readonly IDictionary<IQuidjiboContext, Queue<PipelineStep>> _running;

        private readonly IPayloadSerializer _serializer;
        private readonly IList<PipelineStep> _steps;

        public QuidjiboPipeline(
            IList<PipelineStep> steps,
            ILoggerFactory loggerFactory,
            IDependencyResolver resolver,
            IPayloadProtector protector,
            IPayloadSerializer serializer,
            IWorkDispatcher dispatcher)
        {
            _resolver = resolver;
            _protector = protector;
            _serializer = serializer;
            _dispatcher = dispatcher;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<QuidjiboPipeline>();
            _steps = steps;
            _running = new ConcurrentDictionary<IQuidjiboContext, Queue<PipelineStep>>();
        }

        public async Task StartAsync(IQuidjiboContext context, CancellationToken cancellationToken)
        {
            _running.Add(context, new Queue<PipelineStep>(_steps.Where(x => x != null)
                                                                .Select(x => new PipelineStep
                                                                {
                                                                    Type = x.Type,
                                                                    Instance = x.Instance
                                                                })));
            using (_resolver.Begin())
            {
                context.Protector = _protector;
                context.Dispatcher = _dispatcher;
                context.LoggerFactory = _loggerFactory;
                context.Serializer = _serializer;
                context.Resolver = _resolver;

                await InvokeAsync(context, cancellationToken);
            }
        }

        public Task InvokeAsync(IQuidjiboContext context, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug("The pipeline was canceled.");
                return Task.CompletedTask;
            }

            _running.TryGetValue(context, out var steps);
            if (steps == null || steps.Count == 0)
            {
                _logger.LogDebug("The pipeline is complete.");
                return Task.CompletedTask;
            }

            var step = steps.Dequeue();
            if (step.Instance == null)
            {
                _logger.LogDebug("resolving {0}", step.Type);
                step.Instance = (IQuidjiboMiddleware)context.Resolver.Resolve(step.Type);
            }

            return step.Instance.InvokeAsync(context, () => InvokeAsync(context, cancellationToken), cancellationToken);
        }

        public Task EndAsync(IQuidjiboContext context)
        {
            _running.Remove(context);
            return Task.CompletedTask;
        }
    }
}