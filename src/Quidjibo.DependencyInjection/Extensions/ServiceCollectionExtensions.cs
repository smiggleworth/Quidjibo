﻿using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Quidjibo.Clients;
using Quidjibo.Handlers;
using Quidjibo.Misc;

namespace Quidjibo.DependencyInjection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQuidjibo(this IServiceCollection serviceCollection, params Assembly[] assemblies)
        {
            var handlerType = typeof(IQuidjiboHandler<>);
            var serviceDescriptors = from a in assemblies
                                     from t in a.GetTypes()
                                     from intf in t.GetTypeInfo().ImplementedInterfaces
                                     where intf.GetTypeInfo().IsGenericType && intf.GetGenericTypeDefinition() == handlerType
                                     select new ServiceDescriptor(intf, t, ServiceLifetime.Transient);

            foreach (var serviceDescriptor in serviceDescriptors)
            {
                serviceCollection.Add(serviceDescriptor);
            }

            serviceCollection.Add(new ServiceDescriptor(typeof(IQuidjiboClient), _ => QuidjiboClient.Instance, ServiceLifetime.Singleton));

            return serviceCollection;
        }
    }
}