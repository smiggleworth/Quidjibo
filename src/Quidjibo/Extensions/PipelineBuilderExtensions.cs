using Quidjibo.Pipeline.Builders;
using Quidjibo.Pipeline.Middleware;

namespace Quidjibo.Extensions
{
    public static class PipelineBuilderExtensions
    {
        /// <summary>
        /// Use this middleware to dispatch commands to the corresponding handlers.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IQuidjiboPipelineBuilder UseHandlers(this IQuidjiboPipelineBuilder builder)
        {
            return builder.Use(new QuidjiboHandlerMiddleware());
        }

        /// <summary>
        /// Use this middleware to unprotect and deserialize the command and add it to the context
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IQuidjiboPipelineBuilder UseUnwrap(this IQuidjiboPipelineBuilder builder)
        {
            return builder.Use(new QuidjiboUnwrapMiddleware());
        }

        /// <summary>
        /// Use this middleware to unprotect,deserialize and dispatch
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IQuidjiboPipelineBuilder UseDefault(this IQuidjiboPipelineBuilder builder)
        {
            return builder.UseUnwrap().UseHandlers();
        }
    }
}