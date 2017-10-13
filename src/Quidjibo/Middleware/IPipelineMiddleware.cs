using System;
using System.Threading;
using System.Threading.Tasks;

namespace Quidjibo.Middleware {
    public interface IPipelineMiddleware
    {
        Task InvokeAsync(IQuidjiboContext context, Func<Task> next, CancellationToken cancellationToken);
    }
}