using System;

namespace Quidjibo {
    public class PipelineStep
    {
        public Type Type { get; set; }
        public IPipelineMiddleware Instance { get; set; }
    }
}