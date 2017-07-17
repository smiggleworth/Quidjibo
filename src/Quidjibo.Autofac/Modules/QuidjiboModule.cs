using System.Linq;
using System.Reflection;
using Autofac;
using Quidjibo.Clients;
using Quidjibo.Handlers;
using Quidjibo.Misc;
using Module = Autofac.Module;

namespace Quidjibo.Autofac.Modules
{
    public sealed class QuidjiboModule : Module
    {
        private readonly Assembly[] _assemblies;

        public QuidjiboModule(params Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(_assemblies).AsClosedTypesOf(typeof(IQuidjiboHandler<>));
            builder.Register(c => (QuidjiboClient)QuidjiboClient.Instance).As<IQuidjiboClient>().SingleInstance();

            var keys = from a in _assemblies
                       from t in a.GetTypes()
                       where t.IsAssignableTo<IQuidjiboClientKey>()
                       select t;
            foreach (var key in keys)
            {
                var keyedInterface = typeof(IQuidjiboClient<>).MakeGenericType(key);
                var keyedClient = typeof(QuidjiboClient<>).MakeGenericType(key);
                builder.Register(c =>keyedClient.GetProperty("Instance").GetValue(null, null)).As(keyedInterface).SingleInstance();
            }

        }
    }
}