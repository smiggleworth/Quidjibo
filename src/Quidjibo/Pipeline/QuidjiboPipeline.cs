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
        private readonly IList<PipelineStep> _steps;

        public QuidjiboPipeline(IList<PipelineStep> steps, IDependencyResolver resolver)
        {
            _resolver = resolver;
            _steps = steps;
        }

        public async Task StartAsync(IQuidjiboContext context, CancellationToken cancellationToken)
        {
            context.Steps = new Queue<PipelineStep>(_steps.Select(x => new PipelineStep
            {
                Type = x.Type,
                Instance = x.Instance
            }));
            using (_resolver.Begin())
            {
                await InvokeAsync(context, cancellationToken);
            }
        }

        public Task InvokeAsync(IQuidjiboContext context, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.CompletedTask;
            }

            if (context.Steps.Count == 0)
            {
                return Task.CompletedTask;
            }

            var step = context.Steps.Dequeue();
            if (step == null)
            {
                return Task.CompletedTask;
            }

            if (step.Instance == null)
            {
                step.Instance = (IPipelineMiddleware)_resolver.Resolve(step.Type);
            }
            return step.Instance.InvokeAsync(context, () => InvokeAsync(context, cancellationToken), cancellationToken);
        }
    }
}