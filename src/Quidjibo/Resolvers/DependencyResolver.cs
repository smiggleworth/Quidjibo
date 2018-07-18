using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Quidjibo.Resolvers
{
    public class DependencyResolver : IDependencyResolver, IDisposable
    {
        private readonly Assembly[] _assemblies;
        private readonly IDictionary<Type, object> _services;

        public DependencyResolver(IDictionary<Type, object> services, params Assembly[] assemblies)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies), "QuidjiboBuilder did not pass a list of assemblies to scan, make sure to call ConfigureAssemblies.");
        }

        public object Resolve(Type type)
        {
            if (_services != null && _services.TryGetValue(type, out var service))
            {
                return service;
            }

            var info = type.GetTypeInfo();

            // find the a single implementation or bust
            var handler = (from a in _assemblies
                           from t in a.GetTypes()
                           where info.IsAssignableFrom(t.GetTypeInfo())
                           select t).SingleOrDefault();

            if (handler == null)
            {
                throw new NullReferenceException("Could not find a handler that matches your command.");
            }

            return Activator.CreateInstance(handler);
        }

        public IDisposable Begin()
        {
            return this;
        }

        public void Dispose()
        {
        }
    }
}