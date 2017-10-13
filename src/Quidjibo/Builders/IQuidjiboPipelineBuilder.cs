using System;
using System.Threading.Tasks;
using Quidjibo.Pipeline.Contexts;
using Quidjibo.Pipeline.Middleware;

namespace Quidjibo.Builders {
    public interface IQuidjiboPipelineBuilder {

        IQuidjiboPipeline Build();
        IQuidjiboPipelineBuilder Use(Func<IQuidjiboContext, Func<Task>, Task> middleware);
        IQuidjiboPipelineBuilder Use<T>(T middleware = null) where T : class, IPipelineMiddleware;
    }
}