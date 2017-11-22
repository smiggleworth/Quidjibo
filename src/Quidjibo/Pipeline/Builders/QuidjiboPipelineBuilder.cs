using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Quidjibo.Extensions;
using Quidjibo.Pipeline.Contexts;
using Quidjibo.Pipeline.Middleware;
using Quidjibo.Pipeline.Misc;
using Quidjibo.Resolvers;

namespace Quidjibo.Pipeline.Builders
{
    public class QuidjiboPipelineBuilder: IQuidjiboPipelineBuilder
    {
        private readonly IList<PipelineStep> _steps = new List<PipelineStep>();

        public IQuidjiboPipeline Build(IDependencyResolver resolver)
        {
            return new QuidjiboPipeline(_steps, resolver);
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
    }
}
