using System.Linq;
using System.Reflection;
using Quidjibo.Clients;
using Quidjibo.Handlers;
using Quidjibo.Misc;
using StructureMap;

namespace Quidjibo.StructureMap.Registries
{
    public class QuidjiboRegistry : Registry
    {
        public QuidjiboRegistry(params Assembly[] assemblies)
        {
            Scan(_ =>
            {
                _.AssembliesFromApplicationBaseDirectory();
                _.ConnectImplementationsToTypesClosing(typeof(IQuidjiboHandler<>));
            });
            For<IQuidjiboClient>().Use(_ => (QuidjiboClient)QuidjiboClient.Instance).Singleton();


            var keys = from a in assemblies
                       from t in a.GetTypes()
                       where typeof(IQuidjiboClientKey).IsAssignableFrom(t)
                       select t;
            foreach (var key in keys)
            {
                var keyedInterface = typeof(IQuidjiboClient<>).MakeGenericType(key);
                var keyedClient = typeof(QuidjiboClient<>).MakeGenericType(key);
                For(keyedInterface).Use(c => keyedClient.GetProperty("Instance").GetValue(null, null)).Singleton();
            }
        }
    }
}