using System.Threading;
using System.Threading.Tasks;
using GenFu.ValueGenerators.Lorem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Quidjibo.Clients;
using Quidjibo.Factories;
using Quidjibo.Models;
using Quidjibo.Providers;
using Quidjibo.Serializers;
using Quidjibo.Tests.Samples;


namespace Quidjibo.Tests.Clients
{
    [TestClass]
    public class QuidjiboClientTests
    {
        private QuidjiboClient _sut;

        private IScheduleProviderFactory _scheduleProviderFactory;
        private IScheduleProvider _scheduleProvider;
        private IWorkProviderFactory _workProviderFactory;
        private IWorkProvider _workProvider;
        private IPayloadSerializer _payloadSerializer;
        private ICronProvider _cronProvider;

        [TestInitialize]
        public void Init()
        {
            _scheduleProviderFactory = Substitute.For<IScheduleProviderFactory>();
            _scheduleProvider = Substitute.For<IScheduleProvider>();
            _workProviderFactory = Substitute.For<IWorkProviderFactory>();
            _workProvider = Substitute.For<IWorkProvider>();
            _payloadSerializer = Substitute.For<IPayloadSerializer>();
            _cronProvider = Substitute.For<ICronProvider>();
            _sut = new QuidjiboClient(
                _workProviderFactory,
                _scheduleProviderFactory,
                _payloadSerializer,
                _cronProvider);
            _sut.Clear();
        }

        [TestMethod]
        public async Task PublishAsync()
        {
            // Arrange
            var queueName = "default";
            var command = new BasicCommand();
            var delay = 0;
            var cancellationToken = CancellationToken.None;
            _payloadSerializer.SerializeAsync(command, cancellationToken).Returns(Task.FromResult<byte[]>(null));
            _workProviderFactory.CreateAsync(queueName, cancellationToken).Returns(Task.FromResult(_workProvider));

            // Act 
            await _sut.PublishAsync(command, cancellationToken);

            // Assert
            await _workProvider.Received(1).SendAsync(Arg.Is<WorkItem>(x => x.Queue == queueName), delay, cancellationToken);
        }

        [TestMethod]
        public async Task PublishAsync_WithQueueNameAttribute()
        {
            // Arrange
            var queueName = "custom-queue-name";
            var command = new BasicWithAttributeCommand();
            var delay = 0;
            var cancellationToken = CancellationToken.None;
            _payloadSerializer.SerializeAsync(command, cancellationToken).Returns(Task.FromResult<byte[]>(null));
            _workProviderFactory.CreateAsync(queueName, cancellationToken).Returns(Task.FromResult(_workProvider));

            // Act 
            await _sut.PublishAsync(command, cancellationToken);

            // Assert
            await _workProvider.Received(1).SendAsync(Arg.Is<WorkItem>(x => x.Queue == queueName), delay, cancellationToken);
        }


        [TestMethod]
        public async Task PublishAsync_WithDelay()
        {
            // Arrange
            var queueName = "default";
            var command = new BasicCommand();
            var delay = GenFu.GenFu.Random.Next();
            var cancellationToken = CancellationToken.None;
            _payloadSerializer.SerializeAsync(command, cancellationToken).Returns(Task.FromResult<byte[]>(null));
            _workProviderFactory.CreateAsync(queueName, cancellationToken).Returns(Task.FromResult(_workProvider));

            // Act 
            await _sut.PublishAsync(command, delay, cancellationToken);

            // Assert
            await _workProvider.Received(1).SendAsync(Arg.Is<WorkItem>(x => x.Queue == queueName), delay, cancellationToken);
        }

        [TestMethod]
        public async Task PublishAsync_WithQueueName()
        {
            // Arrange
            var queueName = Lorem.Word();
            var command = new BasicCommand();
            var delay = 0;
            var cancellationToken = CancellationToken.None;
            _payloadSerializer.SerializeAsync(command, cancellationToken).Returns(Task.FromResult<byte[]>(null));
            _workProviderFactory.CreateAsync(queueName, cancellationToken).Returns(Task.FromResult(_workProvider));

            // Act 
            await _sut.PublishAsync(command, queueName, cancellationToken);

            // Assert
            await _workProvider.Received(1).SendAsync(Arg.Is<WorkItem>(x => x.Queue == queueName), delay, cancellationToken);
        }

        [TestMethod]
        public async Task PublishAsync_WithDelayAndQueueName()
        {
            // Arrange
            var queueName = Lorem.Word();
            var command = new BasicCommand();
            var delay = GenFu.GenFu.Random.Next();
            var cancellationToken = CancellationToken.None;
            _payloadSerializer.SerializeAsync(command, cancellationToken).Returns(Task.FromResult<byte[]>(null));
            _workProviderFactory.CreateAsync(queueName, cancellationToken).Returns(Task.FromResult(_workProvider));

            // Act 
            await _sut.PublishAsync(command, queueName, delay, cancellationToken);

            // Assert
            await _workProvider.Received(1).SendAsync(Arg.Is<WorkItem>(x => x.Queue == queueName), delay, cancellationToken);
        }

        [TestMethod]
        public async Task ScheduleAsync()
        {
            // Arrange
            var name = Lorem.Word();
            var queueName = "default";
            var cron = Cron.Daily();
            var command = new BasicCommand();
            var cancellationToken = CancellationToken.None;
            _payloadSerializer.SerializeAsync(command, cancellationToken).Returns(Task.FromResult<byte[]>(null));
            _scheduleProviderFactory.CreateAsync(queueName, cancellationToken).Returns(Task.FromResult(_scheduleProvider));

            // Act 
            await _sut.ScheduleAsync(name, command, cron, cancellationToken);

            // Assert
            await _scheduleProvider.Received(1).CreateAsync(Arg.Is<ScheduleItem>(x => x.Queue == queueName && x.CronExpression == cron.Expression), cancellationToken);
        }

        [TestMethod]
        public async Task ScheduleAsync_WithQueueName()
        {
            // Arrange
            var name = Lorem.Word();
            var queueName = Lorem.Word();
            var cron = Cron.Weekly();
            var command = new BasicCommand();
            var cancellationToken = CancellationToken.None;
            _payloadSerializer.SerializeAsync(command, cancellationToken).Returns(Task.FromResult<byte[]>(null));
            _scheduleProviderFactory.CreateAsync(queueName, cancellationToken).Returns(Task.FromResult(_scheduleProvider));

            // Act 
            await _sut.ScheduleAsync(name, queueName, command, cron, cancellationToken);

            // Assert
            await _scheduleProvider.Received(1).CreateAsync(Arg.Is<ScheduleItem>(x => x.Queue == queueName && x.CronExpression == cron.Expression), cancellationToken);
        }

        [TestMethod]
        public async Task ScheduleAsync_WithQueueNameAttribute()
        {
            // Arrange
            var name = Lorem.Word();
            var queueName = "custom-queue-name";
            var cron = Cron.Weekly();
            var command = new BasicWithAttributeCommand();
            var cancellationToken = CancellationToken.None;
            _payloadSerializer.SerializeAsync(command, cancellationToken).Returns(Task.FromResult<byte[]>(null));
            _scheduleProviderFactory.CreateAsync(queueName, cancellationToken).Returns(Task.FromResult(_scheduleProvider));

            // Act 
            await _sut.ScheduleAsync(name, command, cron, cancellationToken);

            // Assert
            await _scheduleProvider.Received(1).CreateAsync(Arg.Is<ScheduleItem>(x => x.Queue == queueName && x.CronExpression == cron.Expression), cancellationToken);
        }
    }
}
