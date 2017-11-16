using Quidjibo.Pipeline.Builders;
using Quidjibo.Pipeline.Middleware;

namespace Quidjibo.Extensions
{
    public static class PipelineBuilderExtensions
    {
        public static IQuidjiboPipelineBuilder UseHandlers(this IQuidjiboPipelineBuilder builder)
        {
            return builder.Use<QuidjiboHandlerMiddleware>();
        }
    }
}