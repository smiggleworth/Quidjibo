using System;
using Quidjibo.Pipeline.Middleware;

namespace Quidjibo.Pipeline.Misc
{
    public class PipelineStep
    {
        public Type Type { get; set; }
        public IQuidjiboMiddleware Instance { get; set; }
    }
}