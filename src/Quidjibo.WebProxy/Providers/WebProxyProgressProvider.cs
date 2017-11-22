using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quidjibo.Models;
using Quidjibo.Providers;
using Quidjibo.WebProxy.Clients;
using Quidjibo.WebProxy.Models;
using Quidjibo.WebProxy.Requests;

namespace Quidjibo.WebProxy.Providers
{
    public class WebProxyProgressProvider : IProgressProvider
    {
        private readonly string[] _queues;

        private readonly ILogger _logger;
        private readonly IWebProxyClient _webProxyClient;
        

        public WebProxyProgressProvider(ILogger logger, IWebProxyClient webProxyClient, string[] queues)
        {
            _logger = logger;
            _webProxyClient = webProxyClient;
            _queues = queues;
        }

        public async Task ReportAsync(ProgressItem item, CancellationToken cancellationToken)
        {
            var request = new WebProxyRequest
            {
                Path = "/progress-items",
                Data = new RequestData<ProgressItem>
                {
                    Queues = _queues,
                    Data = item
                }
            };

            var response = await _webProxyClient.PostAsync(request, cancellationToken);
            if(!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Report progress failed.");
                _logger.LogDebug(response.Content);
            }
        }

        public async Task<List<ProgressItem>> LoadByCorrelationIdAsync(Guid correlationId, CancellationToken cancellationToken)
        {
            var request = new WebProxyRequest
            {
                Path = "/progress-items",
                Data =  new RequestData<Guid>
                {
                    Queues = _queues,
                    Data = correlationId
                }
            };

            var response = await _webProxyClient.GetAsync<List<ProgressItem>>(request, cancellationToken);
            if(!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Load progress failed.");
                _logger.LogDebug(response.Content);
            }
            return response.Data;
        }
    }
}