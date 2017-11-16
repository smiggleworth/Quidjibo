using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Quidjibo.AspNetCore.Middleware
{
    public class QuidjiboWebProxyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public QuidjiboWebProxyMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<QuidjiboWebProxyMiddleware>();

        }


        public async Task Invoke(HttpContext context)
        {
           



           await _next.Invoke(context);



        }
    }
}
