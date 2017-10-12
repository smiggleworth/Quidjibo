using System.Collections.Generic;
using Quidjibo.Models;
using Quidjibo.Providers;

namespace Quidjibo {
    public class QuidjiboContext : IQuidjiboContext
    {
        public Queue<PipelineStep> Steps { get; set; }
        public IQuidjiboProgress Progress { get; set; }
        public IWorkProvider Provider { get; set; }
        public WorkItem Item { get; set; }
    }
}