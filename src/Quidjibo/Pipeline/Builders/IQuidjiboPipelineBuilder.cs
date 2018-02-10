using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quidjibo.Configurations;
using Quidjibo.Dispatchers;
using Quidjibo.Factories;
using Quidjibo.Pipeline.Contexts;
using Quidjibo.Pipeline.Middleware;
using Quidjibo.Protectors;
using Quidjibo.Providers;
using Quidjibo.Resolvers;
using Quidjibo.Serializers;

namespace Quidjibo.Pipeline.Builders
{
    public interface IQuidjiboPipelineBuilder
    {
        /// <summary>
        ///     Use a delegate middleware
        /// </summary>
        /// <param name="middleware"></param>
        /// <returns></returns>
        IQuidjiboPipelineBuilder Use(Func<IQuidjiboContext, Func<Task>, Task> middleware);

        /// <summary>
        ///     Use a implemented middleware
        /// </summary>
        /// <typeparam name="T">The type of middleware</typeparam>
        /// <param name="middleware">
        ///     The instance of the middleware. If this is null a new instance will be resolved each time the
        ///     pipeline is invoked.
        /// </param>
        /// <returns></returns>
        IQuidjiboPipelineBuilder Use<T>(T middleware = null) where T : class, IQuidjiboMiddleware;

        /// <summary>
        /// Build a pipeline to be used by the server.
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="resolver"></param>
        /// <param name="protector"></param>
        /// <param name="serializer"></param>
        /// <param name="dispatcher"></param>
        /// <returns></returns>
        IQuidjiboPipeline Build(
            ILoggerFactory loggerFactory,
            IDependencyResolver resolver,
            IPayloadProtector protector,
            IPayloadSerializer serializer,
            IWorkDispatcher dispatcher);
    }
}