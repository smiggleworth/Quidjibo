using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quidjibo.Pipeline.Contexts;
using Quidjibo.Protectors;
using Quidjibo.Serializers;

namespace Quidjibo.Pipeline.Middleware
{
    /// <summary>
    ///     The handler middleware dispatches the work to the correct handler.
    /// </summary>
    public class QuidjiboUnwrapMiddleware : IQuidjiboMiddleware
    {
        private readonly ILogger _logger;
        private readonly IPayloadProtector _protector;
        private readonly IPayloadSerializer _serializer;

        public QuidjiboUnwrapMiddleware(
            ILoggerFactory loggerFactory,
            IPayloadSerializer serializer,
            IPayloadProtector protector)
        {
            _serializer = serializer;
            _protector = protector;
            _logger = loggerFactory.CreateLogger<QuidjiboHandlerMiddleware>();
        }

        public async Task InvokeAsync(IQuidjiboContext context, Func<Task> next, CancellationToken cancellationToken)
        {
            var payload = await _protector.UnprotectAsync(context.Item.Payload, cancellationToken);
            context.Command = await _serializer.DeserializeAsync(payload, cancellationToken);
            _logger.LogDebug("Set the command on the context.");
            await next();
        }
    }
}