using System;
using Autofac;
using Quidjibo.Resolvers;

namespace Quidjibo.Autofac.Resolvers
{
    public class AutofacDependencyResolver : IDependencyResolver
    {
        private readonly ILifetimeScope _lifetimeScope;
        private ILifetimeScope _nestedLifetimeScope;

        public AutofacDependencyResolver(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public IDisposable Begin()
        {
            _nestedLifetimeScope = _lifetimeScope.BeginLifetimeScope("Quidjibo");
            return _nestedLifetimeScope;
        }

        public object Resolve(Type type)
        {
            return _nestedLifetimeScope.Resolve(type);
        }
    }
}