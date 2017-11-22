using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Quidjibo.AspNetCore.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseQuidjibo(this IApplicationBuilder app, QuidjiboBuilder builder)
        {
            var lifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
            var server = builder.BuildServer();
            builder.BuildClient();
            lifetime.ApplicationStarted.Register(server.Start);
            lifetime.ApplicationStopping.Register(server.Stop);
            lifetime.ApplicationStopped.Register(server.Dispose);
            return app;
        }

        public static IApplicationBuilder UseQuidjibo(this IApplicationBuilder app, Action<QuidjiboBuilder> builder)
        {
            var quidjiboBuilder = new QuidjiboBuilder();

            builder?.Invoke(quidjiboBuilder);

            return app.UseQuidjibo(quidjiboBuilder);
        }
    }
}