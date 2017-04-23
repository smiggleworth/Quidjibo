using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Quidjibo.Clients;
using Quidjibo.Handlers;

namespace Quidjibo.DependencyInjection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQuidjibo(this IServiceCollection serviceCollection, params Assembly[] assemblies)
        {
            var handlerType = typeof(IWorkHandler<>);
            var serviceDescriptors = from a in assemblies
                                     from t in a.GetTypes()
                                     from intf in t.GetTypeInfo().ImplementedInterfaces
                                     where intf.GetTypeInfo().IsGenericType && intf.GetGenericTypeDefinition() == handlerType
                                     select new ServiceDescriptor(intf, t, ServiceLifetime.Transient);

            foreach (var serviceDescriptor in serviceDescriptors)
            {
                serviceCollection.Add(serviceDescriptor);
            }
            serviceCollection.AddSingleton<IQuidjiboClient, QuidjiboClient>();
            return serviceCollection;
        }
    }
}