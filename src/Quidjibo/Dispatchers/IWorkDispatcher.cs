using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Commands;
using Quidjibo.Misc;

namespace Quidjibo.Dispatchers
{
    public interface IWorkDispatcher
    {
        Task DispatchAsync(IQuidjiboCommand command, IQuidjiboProgress progress, CancellationToken cancellationToken);
    }
}