using System.Collections.Generic;
using Quidjibo.Misc;
using Quidjibo.Models;
using Quidjibo.Pipeline.Misc;
using Quidjibo.Providers;
using Quidjibo.Resolvers;

namespace Quidjibo.Pipeline.Contexts
{
    public class QuidjiboContext : IQuidjiboContext
    {
        public Queue<PipelineStep> Steps { get; set; }
        public IQuidjiboProgress Progress { get; set; }
        public IWorkProvider Provider { get; set; }
        public WorkItem Item { get; set; }
        public PipelineState State { get; set; }
    }
}