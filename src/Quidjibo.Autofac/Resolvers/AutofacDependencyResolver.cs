using System;
using System.Threading;
using Autofac;
using Quidjibo.Resolvers;

namespace Quidjibo.Autofac.Resolvers
{
    public class AutofacDependencyResolver : IDependencyResolver
    {
        private readonly ILifetimeScope _lifetimeScope;

        private readonly AsyncLocal<ILifetimeScope> _nestedLifetimeScope = new AsyncLocal<ILifetimeScope>();

        public AutofacDependencyResolver(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public IDisposable Begin()
        {
            return _nestedLifetimeScope.Value = _lifetimeScope.BeginLifetimeScope("Quidjibo");
        }

        public object Resolve(Type type)
        {
            return _nestedLifetimeScope.Value.Resolve(type);
        }
    }
}