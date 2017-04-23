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
            var handlerType = typeof(IWorkHandler<>);
            var handlerTypes = from a in assemblies
                               from t in a.GetTypes()
                               where t.IsAssignableToGenericType(handlerType)
                               select t;

            foreach (var type in handlerTypes)
            {
                var interfaceType = type.GetTypeInfo().ImplementedInterfaces.First(t => handlerType.GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()));
                var genericType = typeof(IWorkHandler<>).MakeGenericType(interfaceType.GenericTypeArguments);
                serviceCollection.Add(new ServiceDescriptor(genericType, type, ServiceLifetime.Transient));
            }
            serviceCollection.AddSingleton<IQuidjiboClient, QuidjiboClient>();
            return serviceCollection;
        }

        /// <summary>
        ///     Determines whether [is assignable to generic type] [the specified generic type].
        ///     http://stackoverflow.com/questions/74616/how-to-detect-if-type-is-another-generic-type
        /// </summary>
        /// <param name="givenType">Type of the given.</param>
        /// <param name="genericType">Type of the generic.</param>
        /// <returns></returns>
        private static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            var typeInfo = givenType.GetTypeInfo();
            var interfaceTypes = typeInfo.ImplementedInterfaces;
            if (interfaceTypes.Any(it => it.GetTypeInfo().IsGenericType && it.GetGenericTypeDefinition() == genericType))
            {
                return true;
            }
            if (typeInfo.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            var baseType = typeInfo.BaseType;
            if (baseType == null)
            {
                return false;
            }

            return IsAssignableToGenericType(baseType, genericType);
        }
    }
}