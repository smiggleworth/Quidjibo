using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Quidjibo.AspNetCore.WebProxy.Deserializers;
using Quidjibo.AspNetCore.WebProxy.Models;
using Quidjibo.AspNetCore.WebProxy.Providers;
using Quidjibo.Models;
using Quidjibo.WebProxy.Requests;

namespace Quidjibo.AspNetCore.WebProxy.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseQuidjiboWebProxy(this IApplicationBuilder app, QuidjiboBuilder quidjiboBuilder)
        {
            app.Map("/quidjibo", quidjibo =>
            {
                quidjibo.Map("/progress-items", progress =>
                {
                    progress.MapWhen(x => x.Request.Method == HttpMethods.Get, get => get.Run(async context =>
                    {
                        await ExecuteAsync<Guid>(context, async wrapper =>
                        {
                            var factory = quidjiboBuilder.ProgressProviderFactory;
                            var provider = await factory.CreateAsync(wrapper.Queues, context.RequestAborted);
                            var data = await provider.LoadByCorrelationIdAsync(wrapper.Data, context.RequestAborted);
                            await WriteAsync(context, data);
                        });
                    }));
                    progress.MapWhen(x => x.Request.Method == HttpMethods.Post, post => post.Run(async context =>
                    {
                        await ExecuteAsync<ProgressItem>(context, async wrapper =>
                        {
                            var factory = quidjiboBuilder.ProgressProviderFactory;
                            var provider = await factory.CreateAsync(wrapper.Queues, context.RequestAborted);
                            await provider.ReportAsync(wrapper.Data, context.RequestAborted);
                            await WriteAsync(context);
                        });
                    }));
                });


                quidjibo.Map("/schedule-items", schedule =>
                {
                    schedule.Map("/receive", receive => receive.Run(async context =>
                    {
                        await ExecuteAsync(context, async wrapper =>
                        {
                            var factory = quidjiboBuilder.ScheduleProviderFactory;
                            var provider = await factory.CreateAsync(wrapper.Queues, context.RequestAborted);
                            var data = await provider.ReceiveAsync(context.RequestAborted);
                            await WriteAsync(context, data);
                        });
                    }));
                    schedule.Map("/complete", receive => receive.Run(async context =>
                    {
                        await ExecuteAsync<ScheduleItem>(context, async wrapper =>
                        {
                            var factory = quidjiboBuilder.ScheduleProviderFactory;
                            var provider = await factory.CreateAsync(wrapper.Queues, context.RequestAborted);
                            await provider.CompleteAsync(wrapper.Data, context.RequestAborted);
                            await WriteAsync(context);
                        });
                    }));
                    schedule.Map("/exists", receive => receive.Run(async context =>
                    {
                        await ExecuteAsync<string>(context, async wrapper =>
                        {
                            var factory = quidjiboBuilder.ScheduleProviderFactory;
                            var provider = await factory.CreateAsync(wrapper.Queues, context.RequestAborted);
                            var data = await provider.ExistsAsync(wrapper.Data, context.RequestAborted);
                            await WriteAsync(context, data);
                        });
                    }));
                    schedule.MapWhen(x => x.Request.Method == HttpMethods.Get, load => load.Run(async context =>
                    {
                        await ExecuteAsync<string>(context, async wrapper =>
                        {
                            var factory = quidjiboBuilder.ScheduleProviderFactory;
                            var provider = await factory.CreateAsync(wrapper.Queues, context.RequestAborted);
                            var data = await provider.LoadByNameAsync(wrapper.Data, context.RequestAborted);
                            await WriteAsync(context, data);
                        });
                    }));
                    schedule.MapWhen(x => x.Request.Method == HttpMethods.Post, create => create.Run(async context =>
                    {
                        await ExecuteAsync<ScheduleItem>(context, async wrapper =>
                        {
                            var factory = quidjiboBuilder.ScheduleProviderFactory;
                            var provider = await factory.CreateAsync(wrapper.Queues, context.RequestAborted);
                            await provider.CreateAsync(wrapper.Data, context.RequestAborted);
                            await WriteAsync(context);
                        });
                    }));
                    schedule.MapWhen(x => x.Request.Method == HttpMethods.Delete, delete => delete.Run(async context =>
                    {
                        await ExecuteAsync<Guid>(context, async wrapper =>
                        {
                            var factory = quidjiboBuilder.ScheduleProviderFactory;
                            var provider = await factory.CreateAsync(wrapper.Queues, context.RequestAborted);
                            await provider.DeleteAsync(wrapper.Data, context.RequestAborted);
                            await WriteAsync(context);
                        });
                    }));
                });


                quidjibo.Map("/work-items", workApp =>
                {
                    workApp.Map("/receive", receive => receive.Run(async context =>
                    {
                        await ExecuteAsync<string>(context, async wrapper =>
                        {
                            var factory = quidjiboBuilder.WorkProviderFactory;
                            var provider = await factory.CreateAsync(wrapper.Queues, context.RequestAborted);
                            var data = await provider.ReceiveAsync(wrapper.Data, context.RequestAborted);
                            await WriteAsync(context, data);
                        });
                    }));
                    workApp.Map("/renew", renew => renew.Run(async context =>
                    {
                        await ExecuteAsync<WorkItem>(context, async wrapper =>
                        {
                            var factory = quidjiboBuilder.WorkProviderFactory;
                            var provider = await factory.CreateAsync(wrapper.Queues, context.RequestAborted);
                            var data = await provider.RenewAsync(wrapper.Data, context.RequestAborted);
                            await WriteAsync(context, data);
                        });
                    }));
                    workApp.Map("/complete", complete => complete.Run(async context =>
                    {
                        await ExecuteAsync<WorkItem>(context, async wrapper =>
                        {
                            var factory = quidjiboBuilder.WorkProviderFactory;
                            var provider = await factory.CreateAsync(wrapper.Queues, context.RequestAborted);
                            await provider.CompleteAsync(wrapper.Data, context.RequestAborted);
                            await WriteAsync(context);
                        });
                    }));
                    workApp.Map("/fault", fault => fault.Run(async context =>
                    {
                        await ExecuteAsync<WorkItem>(context, async wrapper =>
                        {
                            var factory = quidjiboBuilder.WorkProviderFactory;
                            var provider = await factory.CreateAsync(wrapper.Queues, context.RequestAborted);
                            await provider.FaultAsync(wrapper.Data, context.RequestAborted);
                            await WriteAsync(context);
                        });
                    }));
                    workApp.MapWhen(x => x.Request.Method == HttpMethods.Post, send => send.Run(async context =>
                    {
                        await ExecuteAsync<WorkItem>(context, async wrapper =>
                        {
                            var factory = quidjiboBuilder.WorkProviderFactory;
                            var provider = await factory.CreateAsync(wrapper.Queues, context.RequestAborted);
                            await provider.SendAsync(wrapper.Data, 0, context.RequestAborted);
                            await WriteAsync(context);
                        });
                    }));
                });
            });

            return app;
        }

        private static Task ExecuteAsync(HttpContext context, Func<RequestData, Task> func)
        {
            return ExecuteAsync<object>(context, func);
        }

        private static async Task ExecuteAsync<T>(HttpContext context, Func<RequestData<T>, Task> func)
        {
            // get the Authorization header
            var credential = GetClientSecret(context);
            if (credential == null)
            {
                // unauthorized
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            var wrapper = await ParseRequestAsync<T>(context);
            var provider = context.RequestServices.GetRequiredService<IQuidjiboAuthProvider>();

            var authenticated = await provider.AuthenticateAsync(credential.ClientId, credential.ClientSecret, wrapper.Queues);
            if (!authenticated)
            {
                // unauthorized
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            await func(wrapper);
        }

        private static async Task<RequestData<T>> ParseRequestAsync<T>(HttpContext context)
        {
            if (context.Request.Method == HttpMethods.Get)
            {
                return Deserialize<RequestData<T>>(context.Request.Query);
            }
            if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put)
            {
                var body = await ReadBodyAsync(context.Request);
                return JsonConvert.DeserializeObject<RequestData<T>>(body);
            }
            return null;
        }

        private static async Task WriteAsync(HttpContext context, object data = null, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";
            if (data != null)
            {
                await context.Response.WriteAsync(JsonConvert.SerializeObject(data));
            }
        }

        private static async Task<string> ReadBodyAsync(HttpRequest request)
        {
            using (var reader = new StreamReader(request.Body, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private static QuidjiboCredential GetClientSecret(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("Authorization", out var authorization))
            {
                var parts = authorization[0].Split(' ');
                if (parts.Length == 2 && parts[0] == "Quidjibo")
                {
                    var creds = Encoding.UTF8.GetString(Convert.FromBase64String(parts[1]))?.Split(':');
                    if (creds != null && creds.Length == 2)
                    {
                        return new QuidjiboCredential
                        {
                            ClientId = creds[0],
                            ClientSecret = creds[1]
                        };
                    }
                }
            }

            return null;
        }

        private static T Deserialize<T>(IQueryCollection queryCollection)
        {
            return (T)DeserializeInternal(typeof(T), queryCollection);
        }

        private static object DeserializeInternal(Type type, IQueryCollection items, string key = null)
        {
            if (type.IsTraversable(out var targetType))
            {
                if (targetType.IsEnumerable(out var enumerableType))
                {
                    var enumerable = items.Where(x => key != null && x.Key.StartsWith(key))
                        .Select(x => x.Key.Split(new[] { '[', ']' }, 3)[1]).Distinct()
                        .Select(x => DeserializeInternal(enumerableType, items, key != null ? $"{key}[{x}]" : $"[{x}]"))
                        .ToArray();

                    if (type.IsArray)
                    {
                        var arr = (Array)Activator.CreateInstance(type, enumerable.Length);
                        for (var i = 0; i < enumerable.Length; i++)
                        {
                            arr.SetValue(enumerable[i], i);
                        }

                        return arr;
                    }

                    var collectionType = typeof(Collection<>).MakeGenericType(enumerableType);
                    var collection = Activator.CreateInstance(collectionType);
                    for (var i = 0; i < enumerable.Length; i++)
                    {
                        collectionType.GetMethod("Insert")?.Invoke(collection, new[] { i, enumerable[i] });
                    }
                    return collection;
                }

                var obj = Activator.CreateInstance(type);
                var props = type.GetProperties();
                foreach (var prop in props)
                {
                    var propKey = key != null ? $"{key}.{prop.Name}" : prop.Name;
                    var value = DeserializeInternal(prop.PropertyType, items, propKey);
                    prop.SetValue(obj, value);
                }

                return obj;
            }

            if (!string.IsNullOrWhiteSpace(key) && items.ContainsKey(key))
            {
                if (targetType == typeof(Guid))
                {
                    return new Guid(items[key].ToString());
                }

                return Convert.ChangeType(items[key].ToString(), targetType);
            }

            if (targetType.IsValueType)
            {
                return Activator.CreateInstance(targetType);
            }

            return null;
        }
    }
}