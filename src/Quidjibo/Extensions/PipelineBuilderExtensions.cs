using Quidjibo.Middleware;

namespace Quidjibo {
    public static class PipelineBuilderExtensions
    {
        public static IPipelineBuilder UseDispatch(this IPipelineBuilder builder)
        {
            return builder.Use<QuidjiboHandlerMiddleware>();
        }
    }
}