using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Quidjibo.Servers;

namespace Quidjibo.AspNetCore.Extensions
{
    public static class QuidjiboApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseQuidjibo(this IApplicationBuilder app, QuidjiboBuilder quidjiboBuilder)
        {
            var lifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
            var server = quidjiboBuilder.BuildServer();
            quidjiboBuilder.BuildClient();
            lifetime.ApplicationStarted.Register(server.Start);
            lifetime.ApplicationStopping.Register(server.Stop);
            lifetime.ApplicationStopped.Register(server.Dispose);
            return app;
        }
    }
}