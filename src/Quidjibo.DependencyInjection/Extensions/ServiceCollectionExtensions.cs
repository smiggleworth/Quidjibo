using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Quidjibo.Clients;
using Quidjibo.Handlers;

namespace Quidjibo.DependencyInjection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQuidjibo(this IServiceCollection serviceCollection, params Assembly[] assemblies)
        {
            var handlerTypes = from a in assemblies
                               from t in a.GetTypes()
                               where t.IsClosedGenericOf(typeof(IWorkHandler<>))
                               select t;

            foreach (var type in handlerTypes)
            {
                serviceCollection.Add(new ServiceDescriptor(typeof(IWorkHandler<>), type, ServiceLifetime.Transient));
            }
            serviceCollection.AddSingleton<IQuidjiboClient, QuidjiboClient>();
            return serviceCollection;
        }

        private static bool IsClosedGenericOf(this Type type, Type openGeneric)
        {
            return false;
        }
    }
}