using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Pipeline.Contexts;
using Quidjibo.Pipeline.Middleware;
using Quidjibo.Pipeline.Misc;
using Quidjibo.Resolvers;

namespace Quidjibo.Pipeline
{
    public class QuidjiboPipeline : IQuidjiboPipeline
    {
        private readonly IDependencyResolver _resolver;
        private readonly IDictionary<IQuidjiboContext, Queue<PipelineStep>> _running;
        private readonly IList<PipelineStep> _steps;

        public QuidjiboPipeline(IList<PipelineStep> steps, IDependencyResolver resolver)
        {
            _resolver = resolver;
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
            using(_resolver.Begin())
            {
                await InvokeAsync(context, cancellationToken);
            }
        }

        public Task InvokeAsync(IQuidjiboContext context, CancellationToken cancellationToken)
        {
            if(cancellationToken.IsCancellationRequested)
            {
                return Task.CompletedTask;
            }
            _running.TryGetValue(context, out var steps);
            if(steps == null || steps.Count == 0)
            {
                return Task.CompletedTask;
            }
            var step = steps.Dequeue();
            if(step.Instance == null)
            {
                step.Instance = (IQuidjiboMiddleware)_resolver.Resolve(step.Type);
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