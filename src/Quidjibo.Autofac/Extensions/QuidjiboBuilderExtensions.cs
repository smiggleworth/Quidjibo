using Autofac;
using Quidjibo.Autofac.Resolvers;

namespace Quidjibo.Autofac.Extensions
{
    public static class QuidjiboBuilderExtensions
    {
        public static QuidjiboBuilder UseAutofac(this QuidjiboBuilder builder, IContainer container)
        {
            return builder.ConfigureResolver(new AutofacDependencyResolver(container));
        }
    }
}