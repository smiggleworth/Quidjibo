using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Commands;
using Quidjibo.Exceptions;
using Quidjibo.Models;

namespace Quidjibo.Clients
{
    /// <summary>
    ///     A dumb client for odd situations where you have dependency graph that needs the client before the client is ready. This
    ///     allows the object graph to be resolved but will throw if you try to invoke a method.
    /// </summary>
    internal class QuidjiboDumbClient<TKey> : IQuidjiboClient<TKey>
        where TKey : IQuidjiboClientKey
    {
        public void Dispose() { }

        public Task PublishAsync(IQuidjiboCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new QuidjiboNotInitializedException();
        }

        public Task PublishAsync(IQuidjiboCommand command, int delay, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new QuidjiboNotInitializedException();
        }

        public Task PublishAsync(IQuidjiboCommand command, string queueName, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new QuidjiboNotInitializedException();
        }

        public Task PublishAsync(IQuidjiboCommand command, string queueName, int delay, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new QuidjiboNotInitializedException();
        }

        public Task ScheduleAsync(string name, IQuidjiboCommand command, Cron cron, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new QuidjiboNotInitializedException();
        }

        public Task ScheduleAsync(string name, string queue, IQuidjiboCommand command, Cron cron, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new QuidjiboNotInitializedException();
        }
    }
}