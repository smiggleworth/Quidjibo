using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using Quidjibo.Autofac.Extensions;
using Quidjibo.Autofac.Modules;
using Quidjibo.DataProtection.Extensions;
using Quidjibo.EndToEnd.Jobs;
using Quidjibo.EndToEnd.Services;
using Quidjibo.Extensions;
using Quidjibo.Misc;
using Quidjibo.SqlServer.Extensions;

namespace Quidjibo.EndToEnd
{
    internal class Program
    {
        /// <summary>
        ///     This is not how you should store your key.
        /// </summary>
        private static readonly byte[] fakeAesKey = { 140, 52, 131, 108, 237, 60, 103, 138, 79, 217, 220, 226, 228, 192, 105, 56, 239, 39, 69, 247, 82, 55, 152, 94, 130, 99, 171, 120, 96, 247, 158, 216 };

        private static void Main(string[] args)
        {
            var aes = Aes.Create();
            var key = string.Join(",", aes.Key);
            var cts = new CancellationTokenSource();
            MainAsync(args, cts.Token).GetAwaiter().GetResult();
            Console.CancelKeyPress += (s, e) => { cts.Cancel(); };
            cts.Token.WaitHandle.WaitOne();
        }

        private static async Task MainAsync(string[] args, CancellationToken cancellationToken)
        {
            var loggerFactory = new LoggerFactory().AddConsole(LogLevel.Debug);
            var logger = loggerFactory.CreateLogger<Program>();
            logger.LogDebug("Hello Quidjibo!");

            // Setup DI
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new QuidjiboModule(typeof(Program).Assembly));
            containerBuilder.RegisterType<SimpleService>().As<ISimpleService>();
            var container = containerBuilder.Build();

            // Setup Quidjibo
            var quidjiboBuilder = new QuidjiboBuilder()
                                  .ConfigureLogging(loggerFactory)
                                  //.ConfigureAssemblies(typeof(Program).GetTypeInfo().Assembly)
                                  .UseAutofac(container)
                                  .UseAes(fakeAesKey)
                                  .UseSqlServer("Server=localhost;Database=SampleDb;Trusted_Connection=True;")
                                  .ConfigurePipeline(pipeline => pipeline.UseDefault());

            // Quidjibo Client
            var client = quidjiboBuilder.BuildClient();

            var count = 10;
            var list = new List<Task>(count);
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            for (var j = 0; j < count; j++)
            {
                list.Add(client.PublishAsync(new Job.Command(j), cancellationToken));
            }

            await Task.WhenAll(list);
            list.Clear();
            stopWatch.Stop();
            Console.WriteLine("Published {0} items in {1}s", count, stopWatch.Elapsed.TotalSeconds);

            // Quidjibo Server
            using (var workServer = quidjiboBuilder.BuildServer())
            {
                // Start Quidjibo
                workServer.Start();
                cancellationToken.WaitHandle.WaitOne();
            }
        }

        public class CustomKey : IQuidjiboClientKey
        {
        }
    }
}