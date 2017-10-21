using System.Threading;
using System.Threading.Tasks;

namespace Quidjibo.DataProtection.Providers
{
    public interface IKeyProvider
    {
        Task<byte[]> GetKeyAsync(CancellationToken cancellationToken);
    }
}
