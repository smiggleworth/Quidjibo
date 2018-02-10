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
    public class WebProxyWorkProvider : IWorkProvider
    {
        private readonly ILogger _logger;
        private readonly string[] _queues;
        private readonly IWebProxyClient _webProxyClient;

        public WebProxyWorkProvider(ILogger logger, IWebProxyClient webProxyClient, string[] queues)
        {
            _logger = logger;
            _webProxyClient = webProxyClient;
            _queues = queues;
        }

        public async Task SendAsync(WorkItem item, int delay, CancellationToken cancellationToken)
        {
            var request = new WebProxyRequest
            {
                Path = "/work-items",
                Data = new RequestData<WorkItem>
                {
                    Queues = _queues,
                    Data = item
                }
            };
            var response = await _webProxyClient.PostAsync(request, cancellationToken);
            if(!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Send work item failed.");
                _logger.LogDebug(response.Content);
            }
        }

        public async Task<List<WorkItem>> ReceiveAsync(string worker, CancellationToken cancellationToken)
        {
            var request = new WebProxyRequest
            {
                Path = "/work-items/receive",
                Data = new RequestData<string>
                {
                    Queues = _queues,
                    Data = worker
                }
            };
            var response = await _webProxyClient.PostAsync<List<WorkItem>>(request, cancellationToken);
            if(!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Receive work item failed.");
                _logger.LogDebug(response.Content);
            }

            return response.Data;
        }

        public async Task<DateTime> RenewAsync(WorkItem item, CancellationToken cancellationToken)
        {
            var request = new WebProxyRequest
            {
                Path = "/work-items/renew",
                Data = new RequestData<WorkItem>
                {
                    Queues = _queues,
                    Data = item
                }
            };
            var response = await _webProxyClient.PostAsync<DateTime>(request, cancellationToken);
            if(!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Renew work item failed.");
                _logger.LogDebug(response.Content);
            }

            return response.Data;
        }

        public async Task CompleteAsync(WorkItem item, CancellationToken cancellationToken)
        {
            var request = new WebProxyRequest
            {
                Path = "/work-items/complete",
                Data = new RequestData<WorkItem>
                {
                    Queues = _queues,
                    Data = item
                }
            };
            var response = await _webProxyClient.PostAsync(request, cancellationToken);
            if(!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Complete work item failed.");
                _logger.LogDebug(response.Content);
            }
        }

        public async Task FaultAsync(WorkItem item, CancellationToken cancellationToken)
        {
            var request = new WebProxyRequest
            {
                Path = "/work-items/fault",
                Data = new RequestData<WorkItem>
                {
                    Queues = _queues,
                    Data = item
                }
            };
            var response = await _webProxyClient.PostAsync(request, cancellationToken);
            if(!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Fault work item failed.");
                _logger.LogDebug(response.Content);
            }
        }

        public void Dispose()
        {
        }
    }
}