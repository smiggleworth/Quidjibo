using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Factories;
using Quidjibo.Providers;
using Quidjibo.WebProxy.Clients;
using Quidjibo.WebProxy.Providers;

namespace Quidjibo.WebProxy.Factories
{
    public class WebProxyScheduleProviderFactory : IScheduleProviderFactory
    {
        private static readonly SemaphoreSlim SyncLock = new SemaphoreSlim(1, 1);

        private readonly IWebProxyClient _webProxyClient;


        public WebProxyScheduleProviderFactory(IWebProxyClient webProxyClient)
        {
            _webProxyClient = webProxyClient;
        }

        public int PollingInterval => 60;

        public async Task<IScheduleProvider> CreateAsync(string queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await CreateAsync(new[] { queues }, cancellationToken);
        }

        public async Task<IScheduleProvider> CreateAsync(string[] queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await SyncLock.WaitAsync(cancellationToken);
                return await Task.FromResult<IScheduleProvider>(new WebProxyScheduleProvider(_webProxyClient, queues));
            }
            finally
            {
                SyncLock.Release();
            }
        }
    }
}