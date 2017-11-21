using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Factories;
using Quidjibo.Providers;
using Quidjibo.WebProxy.Clients;
using Quidjibo.WebProxy.Providers;

namespace Quidjibo.WebProxy.Factories
{
    public class WebProxyProgressProviderFactory : IProgressProviderFactory
    {
        private static readonly SemaphoreSlim SyncLock = new SemaphoreSlim(1, 1);

        private readonly IWebProxyClient _webProxyClient;
        private IProgressProvider _provider;

        public WebProxyProgressProviderFactory(IWebProxyClient webProxyClient)
        {
            _webProxyClient = webProxyClient;
        }

        public Task<IProgressProvider> CreateAsync(string queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CreateAsync(queues.Split(','), cancellationToken);
        }

        public async Task<IProgressProvider> CreateAsync(string[] queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(_provider != null)
            {
                return _provider;
            }
            try
            {
                await SyncLock.WaitAsync(cancellationToken);
                _provider = new WebProxyProgressProvider(_webProxyClient, queues);
                return _provider;
            }
            finally
            {
                SyncLock.Release();
            }
        }
    }
}