using Autofac;
using Quidjibo.Autofac.Resolvers;
using Quidjibo.Extensions;

namespace Quidjibo.Autofac.Extensions
{
    public static class QuidjiboBuilderExtensions
    {
        public static QuidjiboBuilder UseAutofac(this QuidjiboBuilder builder, IContainer container)
        {
            return builder.ConfigureDispatcher(new AutofacPayloadResolver(container));
        }
    }
}