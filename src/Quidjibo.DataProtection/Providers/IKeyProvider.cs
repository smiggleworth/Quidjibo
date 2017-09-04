using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quidjibo.Providers
{
    public interface IKeyProvider
    {
        Task<byte[]> GetKeyAsync(CancellationToken cancellationToken);
    }
}
