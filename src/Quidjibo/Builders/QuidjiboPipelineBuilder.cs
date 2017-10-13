using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Quidjibo.Extensions;
using Quidjibo.Pipeline;
using Quidjibo.Pipeline.Contexts;
using Quidjibo.Pipeline.Middleware;

namespace Quidjibo.Builders
{
    public class QuidjiboPipelineBuilder: IQuidjiboPipelineBuilder
    {
       
        private readonly IList<PipelineStep> _steps = new List<PipelineStep>();

        public IQuidjiboPipeline Build()
        {
            //return new QuidjiboPipeline(_steps, _resolver());
            return default(QuidjiboPipeline);
        }

        public IQuidjiboPipelineBuilder Use(Func<IQuidjiboContext, Func<Task>, Task> middleware)
        {
            return Use(new PipelineMiddleware(middleware));
        }

        public IQuidjiboPipelineBuilder Use<T>(T middleware = null) where T : class, IPipelineMiddleware
        {
            _steps.Add(new PipelineStep
            {
                Type = typeof(T),
                Instance = middleware
            });
            return this;
        }

        private IQuidjiboPipelineBuilder UseDefaults()
        {

            return this.UseHandlers();
        }
    }
}
