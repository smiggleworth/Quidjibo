using System.Collections.Generic;
using Quidjibo.Misc;
using Quidjibo.Models;
using Quidjibo.Pipeline;
using Quidjibo.Pipeline.Contexts;
using Quidjibo.Providers;
using Quidjibo.Resolvers;

namespace Quidjibo
{
    public class QuidjiboContext : IQuidjiboContext
    {
        public Queue<PipelineStep> Steps { get; set; }
        public IQuidjiboProgress Progress { get; set; }
        public IWorkProvider Provider { get; set; }
        public WorkItem Item { get; set; }
    }
}