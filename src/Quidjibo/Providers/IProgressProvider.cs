using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Models;

namespace Quidjibo.Providers
{
    public interface IProgressProvider
    {
        Task ReportAsync(ProgressItem item, CancellationToken cancellationToken);
    }
}