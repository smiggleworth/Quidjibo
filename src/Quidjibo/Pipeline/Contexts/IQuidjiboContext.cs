using Microsoft.Extensions.Logging;
using Quidjibo.Commands;
using Quidjibo.Dispatchers;
using Quidjibo.Misc;
using Quidjibo.Models;
using Quidjibo.Pipeline.Misc;
using Quidjibo.Protectors;
using Quidjibo.Providers;
using Quidjibo.Resolvers;
using Quidjibo.Serializers;

namespace Quidjibo.Pipeline.Contexts
{
    public interface IQuidjiboContext
    {
        /// <summary>
        ///     Gets or sets the logger factory.
        /// </summary>
        ILoggerFactory LoggerFactory { get; set; }

        /// <summary>
        ///     Gets or sets the progress.
        /// </summary>
        /// <value>
        ///     The progress.
        /// </value>
        IQuidjiboProgress Progress { get; set; }

        /// <summary>
        ///     Gets or sets the provider.
        /// </summary>
        /// <value>
        ///     The provider.
        /// </value>
        IWorkProvider WorkProvider { get; set; }

        /// <summary>
        ///     Gets or sets the item.
        /// </summary>
        /// <value>
        ///     The item.
        /// </value>
        WorkItem Item { get; set; }

        /// <summary>
        ///     Gets or sets the command.
        /// </summary>
        IQuidjiboCommand Command { get; set; }

        /// <summary>
        ///     Gets the state.
        /// </summary>
        /// <value>
        ///     The state.
        /// </value>
        PipelineState State { get; set; }

        /// <summary>
        ///     Gets or sets the resolver.
        /// </summary>
        IDependencyResolver Resolver { get; set; }

        /// <summary>
        ///     Gets or sets the protector.
        /// </summary>
        IPayloadProtector Protector { get; set; }

        /// <summary>
        ///     Gets or sets the serializer.
        /// </summary>
        IPayloadSerializer Serializer { get; set; }

        /// <summary>
        ///     Gets or sets the dispatcher.
        /// </summary>
        IWorkDispatcher Dispatcher { get; set; }
    }
}