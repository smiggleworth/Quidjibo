using System;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Pipeline.Contexts;

namespace Quidjibo.Pipeline.Middleware
{
    public interface IQuidjiboMiddleware
    {
        Task InvokeAsync(IQuidjiboContext context, Func<Task> next, CancellationToken cancellationToken);
    }
}