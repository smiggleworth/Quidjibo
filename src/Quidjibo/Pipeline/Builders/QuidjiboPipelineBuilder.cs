using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quidjibo.Configurations;
using Quidjibo.Dispatchers;
using Quidjibo.Factories;
using Quidjibo.Pipeline.Contexts;
using Quidjibo.Pipeline.Middleware;
using Quidjibo.Pipeline.Misc;
using Quidjibo.Protectors;
using Quidjibo.Providers;
using Quidjibo.Resolvers;
using Quidjibo.Serializers;

namespace Quidjibo.Pipeline.Builders
{
    public class QuidjiboPipelineBuilder : IQuidjiboPipelineBuilder
    {
        private readonly IList<PipelineStep> _steps = new List<PipelineStep>();

        public IQuidjiboPipeline Build(
            ILoggerFactory loggerFactory,
            IDependencyResolver resolver,
            IPayloadProtector protector,
            IPayloadSerializer serializer,
            IWorkDispatcher dispatcher)
        {
            return new QuidjiboPipeline(
                _steps,
                loggerFactory,
                resolver,
                protector,
                serializer,
                dispatcher);
        }

        public IQuidjiboPipelineBuilder Use(Func<IQuidjiboContext, Func<Task>, Task> middleware)
        {
            return Use(new QuidjiboMiddleware(middleware));
        }

        public IQuidjiboPipelineBuilder Use<T>(T middleware = null) where T : class, IQuidjiboMiddleware
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