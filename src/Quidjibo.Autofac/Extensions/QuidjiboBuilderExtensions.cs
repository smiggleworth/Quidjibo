using Autofac;
using Quidjibo.Autofac.Resolvers;

namespace Quidjibo.Autofac.Extensions
{
    public static class QuidjiboBuilderExtensions
    {
        public static QuidjiboBuilder UseAutofac(this QuidjiboBuilder builder, ILifetimeScope lifetimeScope)
        {
            return builder.ConfigureResolver(new AutofacDependencyResolver(lifetimeScope));
        }
    }
}