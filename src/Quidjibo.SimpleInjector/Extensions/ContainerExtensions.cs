using System.Linq;
using System.Reflection;
using Quidjibo.Clients;
using Quidjibo.Handlers;
using Quidjibo.Misc;
using SimpleInjector;

namespace Quidjibo.SimpleInjector.Extensions
{
    public static class ContainerExtensions
    {
        public static void RegisterQuidjibo(this Container container, params Assembly[] assemblies)
        {
            container.Register(typeof(IQuidjiboHandler<>), assemblies);
            container.RegisterSingleton(typeof(IQuidjiboClient), () => (QuidjiboClient)QuidjiboClient.Instance);


            var keys = from a in assemblies
                       from t in a.GetTypes()
                       where typeof(IQuidjiboClientKey).IsAssignableFrom(t)
                       select t;
            foreach (var key in keys)
            {
                var keyedInterface = typeof(IQuidjiboClient<>).MakeGenericType(key);
                var keyedClient = typeof(QuidjiboClient<>).MakeGenericType(key);
                container.RegisterSingleton(keyedInterface, () => keyedClient.GetProperty("Instance").GetValue(null, null));
            }
        }
    }
}