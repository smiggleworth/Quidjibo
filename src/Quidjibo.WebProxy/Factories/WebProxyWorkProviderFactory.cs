using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Factories;
using Quidjibo.Providers;
using Quidjibo.WebProxy.Clients;
using Quidjibo.WebProxy.Providers;

namespace Quidjibo.WebProxy.Factories
{
    public class WebProxyWorkProviderFactory : IWorkProviderFactory
    {
        private static readonly SemaphoreSlim SyncLock = new SemaphoreSlim(1, 1);

        private readonly IWebProxyClient _webProxyClient;

        public WebProxyWorkProviderFactory(IWebProxyClient webProxyClient)
        {
            _webProxyClient = webProxyClient;
        }

        public int PollingInterval => 10;

        public async Task<IWorkProvider> CreateAsync(string queue, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await SyncLock.WaitAsync(cancellationToken);
                var queues = queue.Split(',');
                return new WebProxyWorkProvider(_webProxyClient, queues);
            }
            finally
            {
                SyncLock.Release();
            }
        }
    }
}