using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Middleware;
using Quidjibo.Resolvers;

namespace Quidjibo {
    public class QuidjiboPipeline : IQuidjiboPipeline
    {
        private readonly IPayloadResolver _provider;
        private readonly IList<PipelineStep> _steps;

        public QuidjiboPipeline(IList<PipelineStep> steps, IPayloadResolver provider)
        {
            _provider = provider;
            _steps = steps;
        }

        public async Task StartAsync(IQuidjiboContext context, CancellationToken cancellationToken)
        {
            context.Steps = new Queue<PipelineStep>(_steps);
            using (_provider.Begin())
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
                step.Instance = (IPipelineMiddleware)_provider.Resolve(step.Type);
            }
            return step.Instance.InvokeAsync(context, () => InvokeAsync(context, cancellationToken), cancellationToken);
        }
    }
}