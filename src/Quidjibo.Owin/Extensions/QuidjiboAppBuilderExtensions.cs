using System;
using Microsoft.Owin.BuilderProperties;
using Owin;
using Quidjibo.Servers;

namespace Quidjibo.Owin.Extensions
{
    public static class QuidjiboAppBuilderExtensions
    {
        public static IAppBuilder UseWorkServer(this IAppBuilder appBuilder, Func<IWorkServer> workServer)
        {
            var props = new AppProperties(appBuilder.Properties);
            var server = workServer();
            props.OnAppDisposing.Register(server.Dispose);
            server.Start();
            return appBuilder;
        }
    }
}