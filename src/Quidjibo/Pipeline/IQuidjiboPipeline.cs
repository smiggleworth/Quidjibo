using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quidjibo.Configurations;
using Quidjibo.Dispatchers;
using Quidjibo.Factories;
using Quidjibo.Pipeline.Contexts;
using Quidjibo.Protectors;
using Quidjibo.Providers;
using Quidjibo.Resolvers;
using Quidjibo.Serializers;

namespace Quidjibo.Pipeline
{
    public interface IQuidjiboPipeline
    {

//        ICronProvider CronProvider { get; }
//        IDependencyResolver Resolver { get; }
//        ILoggerFactory LoggerFactory { get; }
//        IPayloadProtector Protector { get; }
//        IPayloadSerializer Serializer { get; }
//        IQuidjiboConfiguration QuidjiboConfiguration { get; }
//        IWorkDispatcher Dispatcher { get; }


        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StartAsync(IQuidjiboContext context, CancellationToken cancellationToken);

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task InvokeAsync(IQuidjiboContext context, CancellationToken cancellationToken);

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task EndAsync(IQuidjiboContext context);
    }
}