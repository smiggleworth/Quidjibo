using System.Threading;
using System.Threading.Tasks;
using Quidjibo.WebProxy.Models;

namespace Quidjibo.WebProxy.Clients
{
    public interface IWebProxyClient
    {
        /// <summary>
        ///     Gets the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<WebProxyResponse> GetAsync(WebProxyRequest request, CancellationToken cancellationToken);

        /// <summary>
        ///     Gets the specified request.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<WebProxyResponse<TModel>> GetAsync<TModel>(WebProxyRequest request, CancellationToken cancellationToken);

        /// <summary>
        ///     Posts the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<WebProxyResponse> PostAsync(WebProxyRequest request, CancellationToken cancellationToken);

        /// <summary>
        ///     Posts the specified request.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<WebProxyResponse<TModel>> PostAsync<TModel>(WebProxyRequest request, CancellationToken cancellationToken);

        /// <summary>
        ///     Puts the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<WebProxyResponse> PutAsync(WebProxyRequest request, CancellationToken cancellationToken);

        /// <summary>
        ///     Puts the specified request.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<WebProxyResponse<TModel>> PutAsync<TModel>(WebProxyRequest request, CancellationToken cancellationToken);

        /// <summary>
        ///     Deletes the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<WebProxyResponse> DeleteAsync(WebProxyRequest request, CancellationToken cancellationToken);

        /// <summary>
        ///     Deletes the specified request.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<WebProxyResponse<TModel>> DeleteAsync<TModel>(WebProxyRequest request, CancellationToken cancellationToken);
    }
}