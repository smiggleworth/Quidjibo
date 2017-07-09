using System;
using System.Linq;
using System.Reflection;

namespace Quidjibo.Resolvers
{
    public class PayloadResolver : IPayloadResolver
    {
        private readonly Assembly[] _assemblies;

        public PayloadResolver(params Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        public object Resolve(Type type)
        {
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
            return default(IDisposable);
        }
    }
}