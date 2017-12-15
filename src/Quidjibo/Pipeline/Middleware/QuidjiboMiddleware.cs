using System;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Pipeline.Contexts;

namespace Quidjibo.Pipeline.Middleware
{
    /// <summary>
    ///     This is the internal middleware created to wrap delegate middleware.
    /// </summary>
    public sealed class QuidjiboMiddleware : IQuidjiboMiddleware
    {
        private readonly Func<IQuidjiboContext, Func<Task>, Task> _func;

        public QuidjiboMiddleware(Func<IQuidjiboContext, Func<Task>, Task> func)
        {
            _func = func;
        }

        public Task InvokeAsync(IQuidjiboContext context, Func<Task> next, CancellationToken cancellationToken)
        {
            return _func.Invoke(context, next);
        }
    }
}