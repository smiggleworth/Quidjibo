using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Quidjibo.Servers;

namespace Quidjibo.AspNet.Extensions
{
    public static class QuidjiboApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWorkServer(this IApplicationBuilder app, Func<IWorkServer> workServer)
        {
            var lifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
            var server = workServer();
            lifetime.ApplicationStarted.Register(server.Start);
            lifetime.ApplicationStopping.Register(server.Stop);
            lifetime.ApplicationStopped.Register(server.Dispose);
            return app;
        }
    }
}