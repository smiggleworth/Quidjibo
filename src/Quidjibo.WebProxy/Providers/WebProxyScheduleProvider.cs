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
    public class WebProxyScheduleProvider : IScheduleProvider
    {
        private readonly string[] _queues;
        private readonly ILogger _logger;
        private readonly IWebProxyClient _webProxyClient;

        public WebProxyScheduleProvider(ILogger logger, IWebProxyClient webProxyClient, string[] queues)
        {
            _logger = logger;
            _webProxyClient = webProxyClient;
            _queues = queues;
        }

        public async Task<List<ScheduleItem>> ReceiveAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new WebProxyRequest
            {
                Path = "/schedule-items/receive",
                Data = new RequestData
                {
                    Queues = _queues
                }
            };

            var response = await _webProxyClient.PostAsync<List<ScheduleItem>>(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Receive schedule items failed.");
                _logger.LogDebug(response.Content);
            }
            return response.Data;
        }

        public async Task CompleteAsync(ScheduleItem item, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new WebProxyRequest
            {
                Path = "/schedule-items/complete",
                Data = new RequestData<ScheduleItem>
                {
                    Queues = _queues,
                    Data = item
                }
            };

            var response = await _webProxyClient.PostAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Complete schedule items failed.");
                _logger.LogDebug(response.Content);
            }
        }

        public async Task CreateAsync(ScheduleItem item, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new WebProxyRequest
            {
                Path = "/schedule-items",
                Data = new RequestData<ScheduleItem>
                {
                    Queues = _queues,
                    Data = item
                }
            };

            var response = await _webProxyClient.PostAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Create schedule item failed.");
                _logger.LogDebug(response.Content);
            }
        }

        public async Task<ScheduleItem> LoadByNameAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new WebProxyRequest
            {
                Path = "/schedule-items",
                Data = new RequestData<string>
                {
                    Queues = _queues,
                    Data = name
                }
            };

            var response = await _webProxyClient.GetAsync<ScheduleItem>(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Load schedule item failed.");
                _logger.LogDebug(response.Content);
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
                Path = "/schedule-items",
                Data = new RequestData<Guid>
                {
                    Queues = _queues,
                    Data = id
                }
            };

            var response = await _webProxyClient.DeleteAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Delete schedule item failed.");
                _logger.LogDebug(response.Content);
            }
        }

        public async Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new WebProxyRequest
            {
                Path = "/schedule-items/exists",
                Data = new RequestData<string>
                {
                    Queues = _queues,
                    Data = name
                }
            };

            var response = await _webProxyClient.GetAsync<bool>(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Schedule item exist check failed.");
                _logger.LogDebug(response.Content);
            }
            return response.Data;
        }
    }
}