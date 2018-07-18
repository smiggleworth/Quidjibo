using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Models;

namespace Quidjibo.Providers
{
    public class ProgressProvider : IProgressProvider
    {
        public Task ReportAsync(ProgressItem item, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<List<ProgressItem>> LoadByCorrelationIdAsync(Guid correlationId, CancellationToken cancellationToken)
        {
            return Task.FromResult(new List<ProgressItem>());
        }
    }
}