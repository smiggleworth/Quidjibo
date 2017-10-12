using System;
using Quidjibo.DependencyInjection.Resolvers;

namespace Quidjibo.DependencyInjection.Extensions
{
    public static class QuidjiboBuilderExtensions
    {
        public static QuidjiboBuilder UseDependencyInjection(this QuidjiboBuilder builder, IServiceProvider serviceProvider)
        {
            return builder.ConfigureResolver(new DependencyInjectionPayloadResolver(serviceProvider));
        }
    }
}