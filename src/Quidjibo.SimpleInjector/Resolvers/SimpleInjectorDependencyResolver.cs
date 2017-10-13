using System;
using Quidjibo.Resolvers;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace Quidjibo.SimpleInjector.Resolvers
{
    public class SimpleInjectorDependencyResolver : IDependencyResolver
    {
        private readonly Container _container;

        public SimpleInjectorDependencyResolver(Container container)
        {
            _container = container;
        }

        public IDisposable Begin()
        {
            return AsyncScopedLifestyle.BeginScope(_container);
        }

        public object Resolve(Type type)
        {
            return _container.GetInstance(type);
        }
    }
}