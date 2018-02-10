using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quidjibo.Extensions;
using Quidjibo.Pipeline.Contexts;

namespace Quidjibo.Pipeline.Middleware
{
    /// <summary>
    ///     The unwrap middleware sets the command on the context
    /// </summary>
    public class QuidjiboUnwrapMiddleware : IQuidjiboMiddleware
    {
        public async Task InvokeAsync(IQuidjiboContext context, Func<Task> next, CancellationToken cancellationToken)
        {
            var logger = context.LoggerFactory.CreateLogger<QuidjiboUnwrapMiddleware>();
            var payload = await context.Protector.UnprotectAsync(context.Item.Payload, cancellationToken);
            context.Command = await context.Serializer.DeserializeAsync(payload, cancellationToken);
            logger.LogDebug("{0} was set on the context.", context.Command.GetQualifiedName());
            await next();
        }
    }
}