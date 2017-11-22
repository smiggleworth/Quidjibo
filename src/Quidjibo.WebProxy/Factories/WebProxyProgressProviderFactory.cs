using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quidjibo.Factories;
using Quidjibo.Providers;
using Quidjibo.WebProxy.Clients;
using Quidjibo.WebProxy.Providers;

namespace Quidjibo.WebProxy.Factories
{
    public class WebProxyProgressProviderFactory : IProgressProviderFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IWebProxyClient _webProxyClient;

        public WebProxyProgressProviderFactory(ILoggerFactory loggerFactory, IWebProxyClient webProxyClient)
        {
            _loggerFactory = loggerFactory;
            _webProxyClient = webProxyClient;
        }

        public Task<IProgressProvider> CreateAsync(string queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CreateAsync(queues.Split(','), cancellationToken);
        }

        public async Task<IProgressProvider> CreateAsync(string[] queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            var provider = new WebProxyProgressProvider(_loggerFactory.CreateLogger<WebProxyProgressProvider>(), _webProxyClient, queues);
            return await Task.FromResult(provider);
        }
    }
}