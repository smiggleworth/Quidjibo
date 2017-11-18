using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Quidjibo.AspNetCore.Providers;

namespace Quidjibo.AspNetCore.Middleware
{
    public class QuidjiboAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public QuidjiboAuthenticationMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<QuidjiboWebProxyMiddleware>();

        }


        public async Task Invoke(HttpContext context, IQuidjiboAuthProvider provider)
        {
            // check the header for quidjibo Authorization 

            var key = "";
            if (!await provider.AuthenticateAsync(key))
            {
                // return
            }



            await _next.Invoke(context);



        }
    }
}