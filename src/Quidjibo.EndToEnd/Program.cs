using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quidjibo.SqlServer.Configurations;
using Quidjibo.DataProtection.Extensions;
using Quidjibo.SqlServer.Extensions;

namespace Quidjibo.EndToEnd
{
    class Program
    {
        /// <summary>
        /// This is not how you should store your key.
        /// </summary>
        static byte[] fakeAesKey = new byte[]{ 140,52,131,108,237,60,103,138,79,217,220,226,228,192,105,56,239,39,69,247,82,55,152,94,130,99,171,120,96,247,158,216};


        static void Main(string[] args)
        {
            var aes = Aes.Create();
            var key = string.Join(",",aes.Key);



            var cts = new CancellationTokenSource();
            MainAsync(args, cts.Token).GetAwaiter().GetResult();
            Console.CancelKeyPress += (s, e) =>
            {
                cts.Cancel();
            };
        }

        static async Task MainAsync(string[] args, CancellationToken cancellationToken)
        {
            var loggerFactory = new LoggerFactory().AddConsole(LogLevel.Debug);

            var logger = loggerFactory.CreateLogger<Program>();

            logger.LogDebug("Hello Quidjibo!");

            var quidjiboBuilder = new QuidjiboBuilder()
                .ConfigureLogging(loggerFactory)
                .ConfigureDispatcher(typeof(Program).GetTypeInfo().Assembly)
                .UseAes(fakeAesKey)
                .UseSqlServer(new SqlServerQuidjiboConfiguration
                {
                    // load your connection string
                    ConnectionString = "Server=localhost;Database=SampleDb;Trusted_Connection=True;",

                    // the queues the worker should be polling
                    Queues = new List<string>
                    {
                        "default",
                        "other"
                    },

                    // the delay between batches
                    PollingInterval = 10,

                    // maximum concurrent requests
                    Throttle = 2,
                    SingleLoop = true
                });

            var client = quidjiboBuilder.BuildClient();
            using (var workServer = quidjiboBuilder.BuildServer())
            {
                workServer.Start();

                var i = 1;
                var random = new Random();
                while (!cancellationToken.IsCancellationRequested)
                {
                    var count = random.Next(1, 50);
                    for (var j = 0; j < count; j++)
                    {
                        await client.PublishAsync(new Job.Command(i), cancellationToken);
                        i++;
                    }

                    var delay = random.Next(1, 120);
                    await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken);
                }
            }
            await Task.CompletedTask;
        }
    }
}