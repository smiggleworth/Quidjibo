using Quidjibo.Clients;
using Quidjibo.Handlers;
using Quidjibo.Misc;
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
                _.ConnectImplementationsToTypesClosing(typeof(IQuidjiboHandler<>));
            });
            For<IQuidjiboClient>().Use(_ => (QuidjiboClient)QuidjiboClient.Instance).Singleton();
        }
    }
}