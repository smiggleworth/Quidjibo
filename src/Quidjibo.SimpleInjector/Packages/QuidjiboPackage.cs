using System.Reflection;
using Quidjibo.Clients;
using Quidjibo.Handlers;
using SimpleInjector;

namespace Quidjibo.SimpleInjector.Packages
{
    public sealed class QuidjiboPackage
    {
        public static void RegisterHandlers(Container container, params Assembly[] assemblies)
        {
            container.Register(typeof(IWorkHandler<>), assemblies);
            container.RegisterSingleton(QuidjiboClient.Instance);
        }
    }
}