using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Commands;
using Quidjibo.Models;

namespace Quidjibo.Clients
{
    /// <summary>
    ///     A dumb client for odd situations where you have dependency graph that needs the client before the client is ready.
    ///     This
    ///     allows the object graph to be resolved but will throw if you try to invoke a method.
    /// </summary>
    internal class QuidjiboDumbClient : IQuidjiboClient
    {
        public void Dispose()
        {
        }

        public Task PublishAsync(IWorkCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new QuidjiboNotInitializedException();
        }

        public Task PublishAsync(IWorkCommand command, int delay, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new QuidjiboNotInitializedException();
        }

        public Task PublishAsync(IWorkCommand command, string queueName, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new QuidjiboNotInitializedException();
        }

        public Task PublishAsync(IWorkCommand command, string queueName, int delay, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new QuidjiboNotInitializedException();
        }

        public Task ScheduleAsync(string name, IWorkCommand command, Cron cron, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new QuidjiboNotInitializedException();
        }

        public Task ScheduleAsync(string name, string queue, IWorkCommand command, Cron cron, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new QuidjiboNotInitializedException();
        }
    }
}