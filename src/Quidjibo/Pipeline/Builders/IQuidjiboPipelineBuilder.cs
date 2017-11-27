using System;
using System.Threading.Tasks;
using Quidjibo.Pipeline.Contexts;
using Quidjibo.Pipeline.Middleware;
using Quidjibo.Resolvers;

namespace Quidjibo.Pipeline.Builders
{
    public interface IQuidjiboPipelineBuilder
    {
        /// <summary>
        ///     Build the pipeline
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        IQuidjiboPipeline Build(IDependencyResolver resolver);

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
    }
}