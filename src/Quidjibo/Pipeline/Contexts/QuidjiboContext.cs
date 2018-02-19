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
    public class QuidjiboContext : IQuidjiboContext
    {
        public ILoggerFactory LoggerFactory { get; set; }
        public IQuidjiboProgress Progress { get; set; }
        public IWorkProvider WorkProvider { get; set; }
        public WorkItem Item { get; set; }
        public IQuidjiboCommand Command { get; set; }
        public PipelineState State { get; set; }
        public IDependencyResolver Resolver { get; set; }
        public IPayloadProtector Protector { get; set; }
        public IPayloadSerializer Serializer { get; set; }
        public IWorkDispatcher Dispatcher { get; set; }
    }
}