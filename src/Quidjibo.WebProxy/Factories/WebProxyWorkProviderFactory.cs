using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quidjibo.Factories;
using Quidjibo.Providers;
using Quidjibo.WebProxy.Clients;
using Quidjibo.WebProxy.Providers;

namespace Quidjibo.WebProxy.Factories
{
    public class WebProxyWorkProviderFactory : IWorkProviderFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IWebProxyClient _webProxyClient;

        public int PollingInterval => 10;

        public WebProxyWorkProviderFactory(ILoggerFactory loggerFactory, IWebProxyClient webProxyClient)
        {
            _loggerFactory = loggerFactory;
            _webProxyClient = webProxyClient;
        }

        public Task<IWorkProvider> CreateAsync(string queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CreateAsync(queues.Split(','), cancellationToken);
        }

        public async Task<IWorkProvider> CreateAsync(string[] queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            var provider = new WebProxyWorkProvider(_loggerFactory.CreateLogger<WebProxyWorkProvider>(), _webProxyClient, queues);
            return await Task.FromResult(provider);
        }
    }
}