using System;

namespace Quidjibo {
    public interface IPipelineServiceProvider
    {
        IDisposable Begin();
        object Resolve(Type type);

    }
}