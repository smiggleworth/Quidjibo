using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quidjibo.SqlServer.Configurations;
using Quidjibo.SqlServer.Extensions;

namespace Quidjibo.EndToEnd
{
    class Program
    {
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            Console.WriteLine("Hello Quidjibo!");
            MainAsync(args, cts.Token).GetAwaiter().GetResult();
            System.Console.CancelKeyPress += (s, e) =>
            {
                cts.Cancel();
            };
        }

        static async Task MainAsync(string[] args, CancellationToken cancellationToken)
        {
            var quidjiboBuilder = new QuidjiboBuilder().ConfigureDispatcher(typeof(Program).GetTypeInfo().Assembly)
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
                while (!cancellationToken.IsCancellationRequested)
                {
                    await client.PublishAsync(new Job.Command(i), cancellationToken);
                    await Task.Delay(10, cancellationToken);
                    i++;
                }
            }
            await Task.CompletedTask;
        }
    }
}