using System;
using Quidjibo.Pipeline.Middleware;

namespace Quidjibo.Pipeline
{
    public class PipelineStep
    {
        public Type Type { get; set; }
        public IPipelineMiddleware Instance { get; set; }
    }
}