using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Providers;

namespace Quidjibo.Factories
{
    public interface IProgressProviderFactory
    {
        Task<IProgressProvider> CreateAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}