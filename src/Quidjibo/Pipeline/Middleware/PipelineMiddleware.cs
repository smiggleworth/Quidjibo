using System;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Pipeline.Contexts;

namespace Quidjibo.Pipeline.Middleware {
    public class PipelineMiddleware : IPipelineMiddleware
    {
        private readonly Func<IQuidjiboContext, Func<Task>, Task> _func;

        public PipelineMiddleware(Func<IQuidjiboContext, Func<Task>, Task> func)
        {
            _func = func;
        }

        public Task InvokeAsync(IQuidjiboContext context, Func<Task> next, CancellationToken cancellationToken)
        {
            return _func.Invoke(context, next);
        }
    }
}