using System;

namespace Quidjibo.Resolvers
{
    public interface IDependencyResolver
    {
        IDisposable Begin();
        object Resolve(Type type);
    }
}