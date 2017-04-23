using System.Reflection;
using Quidjibo.Clients;
using Quidjibo.Handlers;
using SimpleInjector;

namespace Quidjibo.SimpleInjector.Extensions
{
    public static class ContainerExtensions
    {
        public static void RegisterHandlers(this Container container, params Assembly[] assemblies)
        {
            container.Register(typeof(IWorkHandler<>), assemblies);
            container.RegisterSingleton(QuidjiboClient.Instance);
        }
    }
}