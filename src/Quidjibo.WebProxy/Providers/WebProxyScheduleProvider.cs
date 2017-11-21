using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Models;
using Quidjibo.Providers;
using Quidjibo.WebProxy.Clients;
using Quidjibo.WebProxy.Models;

namespace Quidjibo.WebProxy.Providers
{
    public class WebProxyScheduleProvider : IScheduleProvider
    {
        private readonly List<string> _queues;
        private readonly IWebProxyClient _webProxyClient;

        public WebProxyScheduleProvider(IWebProxyClient webProxyClient, List<string> queues)
        {
            _webProxyClient = webProxyClient;
            _queues = queues;
        }

        public async Task<List<ScheduleItem>> ReceiveAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new WebProxyRequest
            {
                Path = "/schedule-items/receive",
                Data = new
                {
                    Queues = _queues
                }
            };

            var response = await _webProxyClient.PostAsync<List<ScheduleItem>>(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                // log
            }
            return response.Data;
        }

        public async Task CompleteAsync(ScheduleItem item, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new WebProxyRequest
            {
                Path = "/schedule-items/complete",
                Data = item
            };

            var response = await _webProxyClient.PostAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                // log
            }
        }

        public async Task CreateAsync(ScheduleItem item, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new WebProxyRequest
            {
                Path = "/schedule-items",
                Data = item
            };

            var response = await _webProxyClient.PostAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                // log
            }
        }

        public async Task<ScheduleItem> LoadByNameAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new WebProxyRequest
            {
                Path = "/schedule-items",
                Data = new
                {
                    Name = name,
                    Queues = _queues
                }
            };

            var response = await _webProxyClient.GetAsync<ScheduleItem>(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                // log
            }
            return response.Data;
        }

        public Task UpdateAsync(ScheduleItem item, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new WebProxyRequest
            {
                Path = $"/schedule-items/{id}"
            };

            var response = await _webProxyClient.DeleteAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                // log
            }
        }

        public async Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new WebProxyRequest
            {
                Path = "/schedule-items/exists",
                Data = new
                {
                    Name = name,
                    Queues = _queues
                }
            };

            var response = await _webProxyClient.GetAsync<bool>(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                // log
            }
            return response.Data;
        }
    }
}