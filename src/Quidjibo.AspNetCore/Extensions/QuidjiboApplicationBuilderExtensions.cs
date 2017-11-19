using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Quidjibo.AspNetCore.Handlers;
using Quidjibo.Factories;
using Quidjibo.Providers;
using Quidjibo.Servers;
using Quidjibo.WebProxy.Requests;

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
                    schedule.Map("/receive", receive => receive.Run(async context =>
                    {
                        var body = await  ReadBodyAsync(context.Request);
                        var input = JsonConvert.DeserializeObject<ReceiveWorkRequest>();
                        var json = "";

                        if (!authenticated)
                        {
                            // unauthorized
                            return;
                        }
                        if (!validated)
                        {
                            // all queues need to be validated
                        }

                        var factory = (IWorkProviderFactory)null;
                        var provider = await factory.CreateAsync(string.Join(",", input.Queues), context.RequestAborted);
                        await provider.ReceiveAsync(input.Worker)
                        await context.Response.WriteAsync(json, token);
                    }));
                    schedule.Map("/complete", receive => receive.Run(context => null));
                    schedule.Map("/exists", receive => receive.Run(context => null));
                    schedule.MapWhen(x => x.Request.Method == HttpMethods.Get, load => load.Run(context => null));
                    schedule.MapWhen(x => x.Request.Method == HttpMethods.Post, create => create.Run(context => null));
                    schedule.MapWhen(x => x.Request.Method == HttpMethods.Delete, delete => delete.Run(context => null));

                });
                quidjibo.Map("/work-items", work =>
                {
                    work.Map("/receive", receive => receive.Run(context => null));
                    work.Map("/renew", renew => renew.Run(context => null));
                    work.Map("/complete", complete => complete.Run(context => null));
                    work.Map("/fault", complete => complete.Run(context => null));
                    work.MapWhen(x => x.Request.Method == HttpMethods.Post, send => send.Run(context => null));
                });
            });

            return app;
        }





        private static async Task<string> ReadBodyAsync(HttpRequest request)
        {
            using (var reader = new StreamReader(request.Body, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}