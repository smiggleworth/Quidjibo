using System.Reflection;
using Quidjibo.Clients;
using Quidjibo.Handlers;
using SimpleInjector;

namespace Quidjibo.SimpleInjector.Extensions
{
    public static class ContainerExtensions
    {
        public static void RegisterQuidjibo(this Container container, params Assembly[] assemblies)
        {
            container.Register(typeof(IQuidjiboHandler<>), assemblies);
            container.RegisterSingleton(typeof(IQuidjiboClient), () => (QuidjiboClient)QuidjiboClient.Instance);
        }
    }
}