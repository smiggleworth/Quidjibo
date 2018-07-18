using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quidjibo.Factories;
using Quidjibo.Providers;
using Quidjibo.WebProxy.Clients;
using Quidjibo.WebProxy.Providers;

namespace Quidjibo.WebProxy.Factories
{
    public class WebProxyScheduleProviderFactory : IScheduleProviderFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IWebProxyClient _webProxyClient;

        public int PollingInterval => 60;

        public WebProxyScheduleProviderFactory(ILoggerFactory loggerFactory, IWebProxyClient webProxyClient)
        {
            _loggerFactory = loggerFactory;
            _webProxyClient = webProxyClient;
        }

        public async Task<IScheduleProvider> CreateAsync(string queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await CreateAsync(new[]
            {
                queues
            }, cancellationToken);
        }

        public async Task<IScheduleProvider> CreateAsync(string[] queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            var provider = new WebProxyScheduleProvider(_loggerFactory.CreateLogger<WebProxyScheduleProvider>(), _webProxyClient, queues);
            return await Task.FromResult(provider);
        }
    }
}