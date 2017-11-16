using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Quidjibo.Dispatchers;
using Quidjibo.Handlers;
using Quidjibo.Misc;
using Quidjibo.Models;
using Quidjibo.Resolvers;
using Quidjibo.Tests.Samples;

namespace Quidjibo.Tests.Dispatchers
{
    [TestClass]
    public class WorkDispatcherTests
    {
        private IDependencyResolver _resolver;

        private WorkDispatcher _sut;

        [TestInitialize]
        public void Init()
        {
            _resolver = Substitute.For<IDependencyResolver>();
            _sut = new WorkDispatcher(_resolver);
        }

        [TestMethod]
        public async Task DispatchAsync_Should_Invoke_ProcessAsync()
        {
            // Arrange
            var command = new BasicCommand();
            var progress = new QuidjiboProgress();
            var handler = Substitute.For<IQuidjiboHandler<BasicCommand>>();
            _resolver.Resolve(typeof(IQuidjiboHandler<BasicCommand>)).Returns(handler);

            // Act
            await _sut.DispatchAsync(command, progress, CancellationToken.None);

            // Assert
            _resolver.Received(1).Resolve(typeof(IQuidjiboHandler<BasicCommand>));
            await handler.Received(1).ProcessAsync(command, progress, CancellationToken.None);
        }
    }
}