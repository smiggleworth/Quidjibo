using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Models;

namespace Quidjibo.Providers
{
    public interface IProgressProvider
    {
        Task ReportAsync(ProgressItem item, CancellationToken cancellationToken);

        Task<List<ProgressItem>> LoadByCorrelationIdAsync(Guid correlationId, CancellationToken cancellationToken);
    }
}