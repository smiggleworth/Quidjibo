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
    public class WebProxyWorkProvider : IWorkProvider
    {
        private readonly string[] _queues;
        private readonly IWebProxyClient _webProxyClient;

        public WebProxyWorkProvider(IWebProxyClient webProxyClient, string[] queues)
        {
            _queues = queues;
            _webProxyClient = webProxyClient;
        }

        public async Task SendAsync(WorkItem item, int delay, CancellationToken cancellationToken)
        {
            var request = new WebProxyRequest
            {
                Path = "/work-items",
                Data = item
            };
            var response = await _webProxyClient.PostAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                // log
            }
        }

        public async Task<List<WorkItem>> ReceiveAsync(string worker, CancellationToken cancellationToken)
        {
            var request = new WebProxyRequest
            {
                Path = "/work-items/receive",
                Data = new 
                {
                    Worker = worker,
                    Queues = _queues
                }
            };
            var response = await _webProxyClient.PostAsync<List<WorkItem>>(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                // log
            }
            return response.Data;
        }

        public async Task<DateTime> RenewAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            var request = new WebProxyRequest
            {
                Path = "/work-items/renew",
                Data = workItem
            };
            var response = await _webProxyClient.PostAsync<DateTime>(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                // log
            }
            return response.Data;
        }

        public async Task CompleteAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            var request = new WebProxyRequest
            {
                Path = "/work-items/complete",
                Data = workItem
            };
            var response = await _webProxyClient.PostAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                // log
            }
        }

        public async Task FaultAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            var request = new WebProxyRequest
            {
                Path = "/work-items/fault",
                Data = workItem
            };
            var response = await _webProxyClient.PostAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                // log
            }
        }

        public void Dispose()
        {
        }
    }
}