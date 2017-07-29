using Microsoft.Owin.BuilderProperties;
using Owin;
using Quidjibo.Owin.Middleware;

namespace Quidjibo.Owin.Extensions
{
    public static class QuidjiboAppBuilderExtensions
    {
        public static IAppBuilder UseQuidjibo(this IAppBuilder appBuilder, QuidjiboBuilder quidjiboBuilder)
        {
            var props = new AppProperties(appBuilder.Properties);
            var server = quidjiboBuilder.BuildServer();
            quidjiboBuilder.BuildClient();
            props.OnAppDisposing.Register(server.Dispose);
            server.Start();
            return appBuilder;
        }

        public static IAppBuilder UseQuidjiboProxyServer(this IAppBuilder appBuilder, QuidjiboBuilder quidjiboBuilder)
        {
            appBuilder.Use<QuidjiboProxyServerMiddleware>(quidjiboBuilder);
            return appBuilder;
        }
    }
}