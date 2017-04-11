using Quidjibo.SimpleInjector.Resolvers;
using SimpleInjector;

namespace Quidjibo.SimpleInjector.Extensions
{
    public static class QuidjiboBuilderExtensions
    {
        public static QuidjiboBuilder UseSimpleInjector(this QuidjiboBuilder builder, Container container)
        {
            return builder.ConfigureDispatcher(new SimpleInjectorPayloadResolver(container));
        }
    }
}