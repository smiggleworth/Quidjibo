using System.Threading.Tasks;

namespace Quidjibo.AspNetCore.WebProxy.Providers
{
    public interface IQuidjiboAuthProvider
    {
        Task<bool> AuthenticateAsync(string clientId, string clientSecret, string[] queues);
    }
}