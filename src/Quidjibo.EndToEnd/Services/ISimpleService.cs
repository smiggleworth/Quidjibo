using System.Threading;
using System.Threading.Tasks;

namespace Quidjibo.EndToEnd.Services
{
    public interface ISimpleService
    {
        Task DoWorkAsync(CancellationToken cancellationToken);
    }
}