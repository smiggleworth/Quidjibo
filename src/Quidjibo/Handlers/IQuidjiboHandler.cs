using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Commands;
using Quidjibo.Misc;

namespace Quidjibo.Handlers
{
    public interface IQuidjiboHandler<in T> where T : IQuidjiboCommand
    {
        Task ProcessAsync(T command, IQuidjiboProgress progress, CancellationToken cancellationToken);
    }
}