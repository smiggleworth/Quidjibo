using System.Collections.Generic;
using Quidjibo.Misc;
using Quidjibo.Models;
using Quidjibo.Providers;
using Quidjibo.Resolvers;

namespace Quidjibo.Pipeline.Contexts
{
    public interface IQuidjiboContext
    {
        Queue<PipelineStep> Steps { get; set; }
        IQuidjiboProgress Progress { get; set; }
        IWorkProvider Provider { get; set; }
        WorkItem Item { get; set; }
    }
}