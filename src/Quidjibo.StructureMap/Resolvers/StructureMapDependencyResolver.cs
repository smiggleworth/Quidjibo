using System;
using System.Threading;
using Quidjibo.Resolvers;
using StructureMap;

namespace Quidjibo.StructureMap.Resolvers
{
    public class StructureMapDependencyResolver : IDependencyResolver
    {
        private readonly IContainer _container;
        private readonly AsyncLocal<IContainer> _nestedLifetimeScope = new AsyncLocal<IContainer>();

        public StructureMapDependencyResolver(IContainer container)
        {
            _container = container;
        }

        public IDisposable Begin()
        {
            return _nestedLifetimeScope.Value = _container.GetNestedContainer();
        }

        public object Resolve(Type type)
        {
            return _nestedLifetimeScope.Value.GetInstance(type);
        }
    }
}