using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Commands;
using Quidjibo.Handlers;
using Quidjibo.Misc;
using Quidjibo.Resolvers;

namespace Quidjibo.Dispatchers
{
    public class WorkDispatcher : IWorkDispatcher
    {
        private readonly IDependencyResolver _resolver;

        public WorkDispatcher(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public async Task DispatchAsync(IQuidjiboCommand command, IQuidjiboProgress progress, CancellationToken cancellationToken)
        {
            var type = typeof(IQuidjiboHandler<>).MakeGenericType(command.GetType());
            var resolved = _resolver.Resolve(type);
            var method = type.GetMethod("ProcessAsync");
            await (Task)method.Invoke(resolved, new object[]
            {
                command,
                progress,
                cancellationToken
            });
        }
    }
}