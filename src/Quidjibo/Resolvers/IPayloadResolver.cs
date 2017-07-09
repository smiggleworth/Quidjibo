using System;

namespace Quidjibo.Resolvers
{
    public interface IPayloadResolver
    {
        IDisposable Begin();
        object Resolve(Type type);
    }
}