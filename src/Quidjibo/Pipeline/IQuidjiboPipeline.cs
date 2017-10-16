using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Pipeline.Contexts;

namespace Quidjibo.Pipeline {
    public interface IQuidjiboPipeline {
        Task StartAsync(IQuidjiboContext context, CancellationToken cancellationToken);
        Task InvokeAsync(IQuidjiboContext context, CancellationToken cancellationToken);
    }
}