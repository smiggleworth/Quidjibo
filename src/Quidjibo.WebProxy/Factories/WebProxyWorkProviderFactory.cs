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

        public Task<IWorkProvider> CreateAsync(string queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CreateAsync(queues.Split(','), cancellationToken);
        }

        public async Task<IWorkProvider> CreateAsync(string[] queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await SyncLock.WaitAsync(cancellationToken);
                return new WebProxyWorkProvider(_webProxyClient, queues);
            }
            finally
            {
                SyncLock.Release();
            }
        }
    }
}