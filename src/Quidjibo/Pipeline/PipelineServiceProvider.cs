using System;

namespace Quidjibo {
    public class PipelineServiceProvider : IPipelineServiceProvider
    {
        public IDisposable Begin()
        {
            return null;
        }

        public object Resolve(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}