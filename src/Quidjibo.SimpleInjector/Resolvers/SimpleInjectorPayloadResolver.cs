using System;
using Quidjibo.Resolvers;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace Quidjibo.SimpleInjector.Resolvers
{
    public class SimpleInjectorPayloadResolver : IPayloadResolver
    {
        private readonly Container _container;
        private Scope _scope;

        public SimpleInjectorPayloadResolver(Container container)
        {
            _container = container;
        }

        public IDisposable Begin()
        {
            _scope = AsyncScopedLifestyle.BeginScope(_container);
            return _scope;
        }

        public object Resolve(Type type)
        {
            return _container.GetInstance(type);
        }
    }
}