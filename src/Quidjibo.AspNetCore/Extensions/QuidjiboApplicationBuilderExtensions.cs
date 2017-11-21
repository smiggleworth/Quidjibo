using System;
using System.IO;
using System.Net;
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
using Quidjibo.Models;
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
                        var queues = await ReadBodyAsync(context.Request);
                        var authenticated = true;
                        if (!authenticated)
                        {
                            // unauthorized
                            return;
                        }
                        var validated = true;
                        if (!validated)
                        {
                            // all queues need to be validated
                        }
                        var factory = (IScheduleProviderFactory)null;
                        var provider = await factory.CreateAsync(queues, context.RequestAborted);
                        var work = await provider.ReceiveAsync(context.RequestAborted);
                        var json = JsonConvert.SerializeObject(work);
                        await context.Response.WriteAsync(json, context.RequestAborted);
                    }));
                    schedule.Map("/complete", receive => receive.Run(async context =>
                    {
                        var body = await ReadBodyAsync(context.Request);
                        var wrapper = JsonConvert.DeserializeObject<RequestWrapper<ScheduleItem>>(body);
                        var authenticated = true;
                        if (!authenticated)
                        {
                            // unauthorized
                            return;
                        }
                        var validated = true;
                        if (!validated)
                        {
                            // all queues need to be validated
                        }
                        var factory = (IScheduleProviderFactory)null;
                        var provider = await factory.CreateAsync(wrapper.GetQueues(), context.RequestAborted);
                        await provider.CompleteAsync(wrapper.Data, context.RequestAborted);
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                    }));
                    schedule.Map("/exists", receive => receive.Run(async context =>
                    {
                        var body = await ReadBodyAsync(context.Request);
                        var wrapper = JsonConvert.DeserializeObject<RequestWrapper<string>>(body);
                        var name = wrapper.Data;
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            await context.Response.WriteAsync("Name is required.");
                            return;
                        }
                        var authenticated = true;
                        if (!authenticated)
                        {
                            // unauthorized
                            return;
                        }
                        var validated = true;
                        if (!validated)
                        {
                            // all queues need to be validated
                        }
                        var factory = (IScheduleProviderFactory)null;
                        var provider = await factory.CreateAsync(string.Join(",", wrapper.Queues), context.RequestAborted);
                        await provider.ExistsAsync(name, context.RequestAborted);
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                    }));
                    schedule.MapWhen(x => x.Request.Method == HttpMethods.Get, load => load.Run(context => null));
                    schedule.MapWhen(x => x.Request.Method == HttpMethods.Post, create => create.Run(context => null));
                    schedule.MapWhen(x => x.Request.Method == HttpMethods.Delete, delete => delete.Run(context => null));

                });
                quidjibo.Map("/work-items", workApp =>
                {
                    workApp.Map("/receive", receive => receive.Run(async context =>
                    {
                        var body = await ReadBodyAsync(context.Request);
                        var input = JsonConvert.DeserializeObject<RequestWrapper<WorkItem>>(body);
                        var authenticated = true;
                        if (!authenticated)
                        {
                            // unauthorized
                            return;
                        }
                        var validated = true;
                        if (!validated)
                        {
                            // all queues need to be validated
                        }
                        var factory = (IWorkProviderFactory)null;
                        var provider = await factory.CreateAsync(string.Join(",", input.Queues), context.RequestAborted);
                        var work = await provider.ReceiveAsync(input.Worker, context.RequestAborted);
                        var json = JsonConvert.SerializeObject(work);
                        await context.Response.WriteAsync(json, context.RequestAborted);
                    }));
                    workApp.Map("/renew", renew => renew.Run(context => { }));
                    workApp.Map("/complete", complete => complete.Run(context => { }));
                    workApp.Map("/fault", complete => complete.Run(context => { }));
                    workApp.MapWhen(x => x.Request.Method == HttpMethods.Post, send => send.Run(context =>
                    {

                    }));
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