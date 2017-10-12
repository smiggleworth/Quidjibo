using System.Collections.Generic;
using Quidjibo.Models;
using Quidjibo.Providers;

namespace Quidjibo {
    public interface IQuidjiboContext
    {
        Queue<PipelineStep> Steps { get; set; }
        IQuidjiboProgress Progress { get; set; }
        IWorkProvider Provider { get; set; }
        WorkItem Item { get; set; }
    }
}