using System.Reflection;
using Quidjibo.Handlers;
using StructureMap;
using StructureMap.TypeRules;

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
        }
    }
}