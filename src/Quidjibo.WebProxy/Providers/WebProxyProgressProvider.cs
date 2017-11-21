using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly IWebProxyClient _webProxyClient;

        public WebProxyProgressProvider(IWebProxyClient webProxyClient, string[] queues)
        {
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
                // log
            }
        }

        public async Task<List<ProgressItem>> LoadByCorrelationIdAsync(Guid correlationId, CancellationToken cancellationToken)
        {
            var request = new WebProxyRequest
            {
                Path = "/progress-items",
                Data = new
                {
                    correlationId
                }
            };

            var response = await _webProxyClient.GetAsync<List<ProgressItem>>(request, cancellationToken);
            if(!response.IsSuccessStatusCode)
            {
                // log
            }
            return response.Data;
        }
    }
}