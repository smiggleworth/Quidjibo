using System.Reflection;
using Autofac;
using Quidjibo.Handlers;
using Module = Autofac.Module;

namespace Quidjibo.Autofac.Modules
{
    public class QuidjiboModule : Module
    {
        private readonly Assembly[] _assemblies;

        public QuidjiboModule(Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(_assemblies).AsClosedTypesOf(typeof(IWorkHandler<>));
        }
    }
}