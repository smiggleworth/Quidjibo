using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Quidjibo.Resolvers;

namespace Quidjibo.DependencyInjection.Resolvers
{
    public class DependencyInjectionDependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly AsyncLocal<IServiceScope> _serviceScope = new AsyncLocal<IServiceScope>();

        public DependencyInjectionDependencyResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IDisposable Begin()
        {
            return _serviceScope.Value = _serviceProvider.CreateScope();
        }

        public object Resolve(Type type)
        {
            return _serviceScope.Value.ServiceProvider.GetService(type);
        }
    }
}