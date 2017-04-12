using Quidjibo.StructureMap.Resolvers;
using StructureMap;

namespace Quidjibo.StructureMap.Extensions
{
    public static class QuidjiboBuilderExtensions
    {
        public static QuidjiboBuilder UseStructureMap(this QuidjiboBuilder builder, IContainer container)
        {
            return builder.ConfigureDispatcher(new StructureMapPayloadResolver(container));
        }
    }
}