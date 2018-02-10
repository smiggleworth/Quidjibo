using System.Threading;
using System.Threading.Tasks;

namespace Quidjibo.EndToEnd.Services
{
    public class SimpleService : ISimpleService
    {
        public Task DoWorkAsync(CancellationToken cancellationToken)
        {
            return Task.Delay(25, cancellationToken);
        }
    }
}