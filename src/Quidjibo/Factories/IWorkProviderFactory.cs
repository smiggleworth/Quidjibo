using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Providers;

namespace Quidjibo.Factories
{
    public interface IWorkProviderFactory
    {
        Task<IWorkProvider> CreateAsync(string queue, CancellationToken cancellationToken = default(CancellationToken));
    }
}