using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Quidjibo.WebProxy.Models;

namespace Quidjibo.WebProxy.Clients
{
    public class WebProxyClient : IWebProxyClient
    {
        private readonly string _clientSecret;
        private readonly string _url;
        private readonly string _clientId;

        public WebProxyClient(string url, string clientId, string clientSecret)
        {
            _url = url;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        /// <summary>
        ///     Gets the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<WebProxyResponse> GetAsync(WebProxyRequest request, CancellationToken cancellationToken)
        {
            var uri = await GetUri(request.Path, request.Data);
            var message = GetHttpRequestMessage(uri, HttpMethod.Get);
            var response = await GetWebProxyResponse(message, cancellationToken);
            return response;
        }

        /// <summary>
        ///     Gets the specified request.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<WebProxyResponse<TModel>> GetAsync<TModel>(WebProxyRequest request, CancellationToken cancellationToken)
        {
            var uri = await GetUri(request.Path, request.Data);
            var message = GetHttpRequestMessage(uri, HttpMethod.Get);
            var response = await GetWebProxyResponse<TModel>(message, cancellationToken);
            return response;
        }

        /// <summary>
        ///     Posts the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<WebProxyResponse> PostAsync(WebProxyRequest request, CancellationToken cancellationToken)
        {
            var uri = await GetUri(request.Path);
            var content = GetJsonContent(request.Data);
            var message = GetHttpRequestMessage(uri, HttpMethod.Post, content);
            var response = await GetWebProxyResponse(message, cancellationToken);
            return response;
        }

        /// <summary>
        ///     Posts the specified request.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<WebProxyResponse<TModel>> PostAsync<TModel>(WebProxyRequest request, CancellationToken cancellationToken)
        {
            var uri = await GetUri(request.Path);
            var content = GetJsonContent(request.Data);
            var message = GetHttpRequestMessage(uri, HttpMethod.Post, content);
            var response = await GetWebProxyResponse<TModel>(message, cancellationToken);
            return response;
        }

        /// <summary>
        ///     Puts the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<WebProxyResponse> PutAsync(WebProxyRequest request, CancellationToken cancellationToken)
        {
            var uri = await GetUri(request.Path);
            var content = GetJsonContent(request.Data);
            var message = GetHttpRequestMessage(uri, HttpMethod.Put, content);
            var response = await GetWebProxyResponse(message, cancellationToken);
            return response;
        }

        /// <summary>
        ///     Puts the specified request.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<WebProxyResponse<TModel>> PutAsync<TModel>(WebProxyRequest request, CancellationToken cancellationToken)
        {
            var uri = await GetUri(request.Path);
            var content = GetJsonContent(request.Data);
            var message = GetHttpRequestMessage(uri, HttpMethod.Put, content);
            var response = await GetWebProxyResponse<TModel>(message, cancellationToken);
            return response;
        }

        /// <summary>
        ///     Deletes the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<WebProxyResponse> DeleteAsync(WebProxyRequest request, CancellationToken cancellationToken)
        {
            var uri = await GetUri(request.Path);
            var message = GetHttpRequestMessage(uri, HttpMethod.Delete);
            var response = await GetWebProxyResponse(message, cancellationToken);
            return response;
        }

        /// <summary>
        ///     Deletes the specified request.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<WebProxyResponse<TModel>> DeleteAsync<TModel>(WebProxyRequest request, CancellationToken cancellationToken)
        {
            var uri = await GetUri(request.Path);
            var message = GetHttpRequestMessage(uri, HttpMethod.Delete);
            var response = await GetWebProxyResponse<TModel>(message, cancellationToken);
            return response;
        }

        internal async Task<WebProxyResponse> GetWebProxyResponse(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            using (var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken))
            {
                var webProxyResponse = new WebProxyResponse
                {
                    Content = await response.Content.ReadAsStringAsync(),
                    IsSuccessStatusCode = response.IsSuccessStatusCode,
                    ReasonPhrase = response.ReasonPhrase,
                    StatusCode = response.StatusCode
                };
                return webProxyResponse;
            }
        }

        internal async Task<WebProxyResponse<T>> GetWebProxyResponse<T>(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            using (var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken))
            {
                var webProxyResponse = new WebProxyResponse<T>
                {
                    Content = await response.Content.ReadAsStringAsync(),
                    IsSuccessStatusCode = response.IsSuccessStatusCode,
                    ReasonPhrase = response.ReasonPhrase,
                    StatusCode = response.StatusCode
                };

                if (response.IsSuccessStatusCode)
                {
                    webProxyResponse.Data = JsonConvert.DeserializeObject<T>(webProxyResponse.Content);
                }
                return webProxyResponse;
            }
        }

        internal HttpRequestMessage GetHttpRequestMessage(Uri uri, HttpMethod method, HttpContent content = null)
        {
            var request = new HttpRequestMessage(method, uri);
            var authentication = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Quidjibo", authentication);
            request.Headers.UserAgent.ParseAdd("quidjibo");
            if (content != null)
            {
                request.Content = content;
            }
            return request;
        }

        internal FormUrlEncodedContent GetFormUrlEncodedContent<T>(T model)
        {
            var keyValuePairs = GetAllKeyValuePairs(model).ToList();
            if (!keyValuePairs.Any())
            {
                return null;
            }

            var content = new FormUrlEncodedContent(keyValuePairs);
            return content;
        }

        internal StringContent GetJsonContent<T>(T model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return content;
        }

        internal async Task<Uri> GetUri(string urlPath)
        {
            return await GetUri<object>(urlPath, null);
        }

        internal async Task<Uri> GetUri<T>(string urlPath, T model)
        {
            var uriBuilder = new UriBuilder(_url);

            uriBuilder.Path = $"{uriBuilder.Path.Trim('/')}/{urlPath.Trim('/')}";

            if (model == null)
            {
                return uriBuilder.Uri;
            }

            var content = GetFormUrlEncodedContent(model);
            if (content == null)
            {
                return uriBuilder.Uri;
            }

            uriBuilder.Query = await content.ReadAsStringAsync();
            return uriBuilder.Uri;
        }

        internal IEnumerable<KeyValuePair<string, string>> GetAllKeyValuePairs<T>(T model)
        {
            return model != null ? GetModelKeyValuePairs(model) : null;
        }

        internal static IEnumerable<KeyValuePair<string, string>> GetModelKeyValuePairs<T>(T model, string parent = null)
        {
            var hasParent = !string.IsNullOrWhiteSpace(parent);
            foreach (var propertyInfo in model.GetType().GetProperties())
            {
                var attributes = propertyInfo.GetCustomAttributes().ToList();
                if (attributes.OfType<JsonIgnoreAttribute>().Any())
                {
                    continue;
                }

                var propertyValue = propertyInfo.GetValue(model);
                if (propertyValue == null)
                {
                    continue;
                }

                var propertyName = propertyInfo.Name;
                var key = hasParent ? $"{parent}.{propertyName}" : propertyName;


                if (Traverse(propertyInfo.PropertyType))
                {
                    if (propertyValue is IEnumerable enumerable)
                    {
                        var i = 0;
                        foreach (var item in enumerable)
                        {
                            var keyi = $"{key}[{i}]";

                            if (Traverse(item.GetType()))
                            {
                                foreach (var child in GetModelKeyValuePairs(item, keyi))
                                {
                                    yield return child;
                                }
                            }
                            else
                            {
                                yield return new KeyValuePair<string, string>(keyi, item.ToString());
                            }

                            i++;
                        }

                        continue;
                    }

                    foreach (var child in GetModelKeyValuePairs(propertyValue, key))
                    {
                        yield return child;
                    }

                    continue;
                }

                yield return new KeyValuePair<string, string>(key, propertyValue.ToString());
            }
        }

        internal static bool Traverse(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = Nullable.GetUnderlyingType(type);
                typeInfo = type.GetTypeInfo();
            }

            return !(type == null || typeInfo.IsNotPublic || type.IsPointer || typeInfo.IsPrimitive
                     || typeof(Action<>).IsAssignableFrom(type)
                     || typeof(bool).IsAssignableFrom(type)
                     || typeof(byte).IsAssignableFrom(type)
                     || typeof(byte[]).IsAssignableFrom(type)
                     || typeof(char).IsAssignableFrom(type)
                     || typeof(char[]).IsAssignableFrom(type)
                     || typeof(DateTime).IsAssignableFrom(type)
                     || typeof(decimal).IsAssignableFrom(type)
                     || typeof(double).IsAssignableFrom(type)
                     || typeof(Exception).IsAssignableFrom(type)
                     || typeof(float).IsAssignableFrom(type)
                     || typeof(Func<>).IsAssignableFrom(type)
                     || typeof(Guid).IsAssignableFrom(type)
                     || typeof(int).IsAssignableFrom(type)
                     || typeof(long).IsAssignableFrom(type)
                     || typeof(MulticastDelegate).IsAssignableFrom(type)
                     || typeof(sbyte).IsAssignableFrom(type)
                     || typeof(short).IsAssignableFrom(type)
                     || typeof(string).IsAssignableFrom(type)
                     || typeof(Task).IsAssignableFrom(type)
                     || typeof(Type).IsAssignableFrom(type)
                     || typeof(uint).IsAssignableFrom(type)
                     || typeof(ulong).IsAssignableFrom(type)
                     || typeof(ushort).IsAssignableFrom(type)
                     || typeof(void).IsAssignableFrom(type)
            );
        }
    }
}