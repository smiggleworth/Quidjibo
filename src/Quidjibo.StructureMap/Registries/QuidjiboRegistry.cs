using Quidjibo.Clients;
using Quidjibo.Handlers;
using StructureMap;

namespace Quidjibo.StructureMap.Registries
{
    public class QuidjiboRegistry : Registry
    {
        public QuidjiboRegistry()
        {
            Scan(_ =>
            {
                _.AssembliesFromApplicationBaseDirectory();
                _.ConnectImplementationsToTypesClosing(typeof(IWorkHandler<>));
            });
            For<IQuidjiboClient>().Use(_ => QuidjiboClient.Instance).Singleton();
        }
    }
}