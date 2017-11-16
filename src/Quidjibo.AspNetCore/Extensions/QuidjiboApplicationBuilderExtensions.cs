using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

        public static IApplicationBuilder UseQuidjiboAuthentication(this IApplicationBuilder app)
        {
            return app;
        }


        public static IApplicationBuilder UseQuidjiboWebProxy(this IApplicationBuilder app, QuidjiboBuilder quidjiboBuilder)
        {
            var lifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();

            app.Map("/quidjibo", quidjibo =>
            {
                quidjibo.UseQuidjiboAuthentication();
                quidjibo.Map("/progress-items", progress =>
                {
                    progress.MapWhen(x => x.Request.Method == HttpMethods.Get, get => get.Run(async context =>
                     {
                         await context.Response.WriteAsync("Get progress items by correlationId.");
                     }));
                    progress.MapWhen(x => x.Request.Method == HttpMethods.Post, post => post.Run(async context =>
                    {
                        await context.Response.WriteAsync("Post progress item.");
                    }));
                });
                quidjibo.Map("/schedule-items", schedule =>
                {

                });
                quidjibo.Map("/work-items", work =>
                {

                });
            });




            return app;
        }



    }
}