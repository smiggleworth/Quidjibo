using System.Threading;
using System.Threading.Tasks;

namespace Quidjibo.Providers
{
    public interface IQueueProvider
    {
        Task CreateAsync(string name, CancellationToken cancellationToken);
        Task GetAsync(string name, CancellationToken cancellationToken);
    }
}