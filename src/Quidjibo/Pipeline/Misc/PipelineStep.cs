using System;
using Quidjibo.Pipeline.Middleware;

namespace Quidjibo.Pipeline.Misc
{
    public class PipelineStep
    {
        public Type Type { get; set; }
        public IPipelineMiddleware Instance { get; set; }
    }

    public class PipelineState
    {
        public bool Success { get; set; } = true;
    }
}