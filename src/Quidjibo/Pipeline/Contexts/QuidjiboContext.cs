using System.Collections.Generic;
using Quidjibo.Misc;
using Quidjibo.Models;
using Quidjibo.Pipeline.Misc;
using Quidjibo.Providers;

namespace Quidjibo.Pipeline.Contexts
{
    public class QuidjiboContext : IQuidjiboContext
    {
        public IQuidjiboProgress Progress { get; set; }
        public IWorkProvider Provider { get; set; }
        public WorkItem Item { get; set; }
        public PipelineState State { get; set; }
    }
}