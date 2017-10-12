using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quidjibo
{
    public class PipelineBuilder 
    {
        private IPipelineServiceProvider _provider = new PipelineServiceProvider();
        private readonly IList<PipelineStep> _steps = new List<PipelineStep>();

        public QuidjiboPipeline Build()
        {
            return new QuidjiboPipeline(_steps, _provider);
        }

        public PipelineBuilder Use(Func<IQuidjiboContext, Func<Task>, Task> middleware)
        {
            return Use(new PipelineMiddleware(middleware));
        }

        public PipelineBuilder Use<T>(T middleware = null) where T : class, IPipelineMiddleware
        {
            _steps.Add(new PipelineStep
            {
                Type = typeof(T),
                Instance = middleware
            });
            return this;
        }

        public PipelineBuilder ConfigurePipelineServiceProvider(IPipelineServiceProvider provider)
        {
            _provider = provider ?? new PipelineServiceProvider();
            return this;
        }
    }
}