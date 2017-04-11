using System.Reflection;
using Quidjibo.Handlers;
using SimpleInjector;

namespace Quidjibo.SimpleInjector.Packages
{
    public sealed class QuidjiboPackage
    {
        public static void RegisterHandlers(Container container, params Assembly[] assemblies)
        {
            container.Register(typeof(IWorkHandler<>), assemblies);
        }
    }
}