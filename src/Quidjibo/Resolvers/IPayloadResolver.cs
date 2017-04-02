using System;

namespace Quidjibo.Resolvers
{
    public interface IPayloadResolver : IDisposable
    {
        IDisposable Begin();
        object Resolve(Type type);
    }
}