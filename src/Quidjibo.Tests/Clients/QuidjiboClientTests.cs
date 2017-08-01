using System;
using System.Threading;
using System.Threading.Tasks;
using GenFu;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Quidjibo.Attributes;
using Quidjibo.Clients;
using Quidjibo.Commands;
using Quidjibo.Factories;
using Quidjibo.Models;
using Quidjibo.Providers;
using Quidjibo.Serializers;
using Quidjibo.Tests.Misc;
using Quidjibo.Tests.Samples;

namespace Quidjibo.Tests.Clients
{
    [TestClass]
    public class QuidjiboClientTests
    {
        private ICronProvider _cronProvider;
        private IPayloadSerializer _payloadSerializer;
        private IScheduleProvider _scheduleProvider;

        private ILoggerFactory _loggerFactory;
        private IScheduleProviderFactory _scheduleProviderFactory;
        private QuidjiboClient _sut;
        private IWorkProvider _workProvider;
        private IWorkProviderFactory _workProviderFactory;

        [TestInitialize]
        public void Init()
        {
            _loggerFactory = Substitute.For<ILoggerFactory>();
            _scheduleProviderFactory = Substitute.For<IScheduleProviderFactory>();
            _scheduleProvider = Substitute.For<IScheduleProvider>();
            _workProviderFactory = Substitute.For<IWorkProviderFactory>();
            _workProvider = Substitute.For<IWorkProvider>();
            _payloadSerializer = Substitute.For<IPayloadSerializer>();
            _cronProvider = Substitute.For<ICronProvider>();
            _sut = new QuidjiboClient(
                _loggerFactory,
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
            var queueName = BaseValueGenerator.Word();
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
            var queueName = BaseValueGenerator.Word();
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
            var name = BaseValueGenerator.Word();
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
            var name = BaseValueGenerator.Word();
            var queueName = BaseValueGenerator.Word();
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
            var name = BaseValueGenerator.Word();
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

        [TestMethod]
        public async Task ScheduleAsync_SkipSchedulesThatAlreadyExists()
        {
            // Arrange
            var name = BaseValueGenerator.Word();
            var queueName = "default";
            var cron = Cron.Weekly();
            var payload = Guid.NewGuid().ToByteArray();
            var command = new BasicCommand();
            var cancellationToken = CancellationToken.None;
            var existingItem = new ScheduleItem
            {
                CronExpression = cron.Expression,
                Name = name,
                Payload = payload,
                Queue = queueName
            };

            _payloadSerializer.SerializeAsync(command, cancellationToken).Returns(Task.FromResult(payload));
            _scheduleProviderFactory.CreateAsync(queueName, cancellationToken).Returns(Task.FromResult(_scheduleProvider));
            _scheduleProvider.LoadByNameAsync(name, cancellationToken).Returns(Task.FromResult(existingItem));

            // Act 
            await _sut.ScheduleAsync(name, command, cron, cancellationToken);

            // Assert
            await _scheduleProvider.DidNotReceiveWithAnyArgs().CreateAsync(default(ScheduleItem), cancellationToken);
        }

        [TestMethod]
        public async Task ScheduleAsync_DeleteSchedulesThatExistButAreDifferent()
        {
            // Arrange
            var name = BaseValueGenerator.Word();
            var queueName = "default";
            var cron = Cron.Weekly();
            var payload = Guid.NewGuid().ToByteArray();
            var command = new BasicCommand();
            var cancellationToken = CancellationToken.None;
            var existingItem = new ScheduleItem
            {
                Id = Guid.NewGuid(),
                CronExpression = cron.Expression,
                Name = name,
                Payload = Guid.NewGuid().ToByteArray(),
                Queue = queueName
            };

            _payloadSerializer.SerializeAsync(command, cancellationToken).Returns(Task.FromResult(payload));
            _scheduleProviderFactory.CreateAsync(queueName, cancellationToken).Returns(Task.FromResult(_scheduleProvider));
            _scheduleProvider.LoadByNameAsync(name, cancellationToken).Returns(Task.FromResult(existingItem));

            // Act 
            await _sut.ScheduleAsync(name, command, cron, cancellationToken);

            // Assert
            await _scheduleProvider.Received(1).DeleteAsync(existingItem.Id, cancellationToken);
            await _scheduleProvider.Received(1).CreateAsync(Arg.Is<ScheduleItem>(x => x.Queue == queueName && x.CronExpression == cron.Expression), cancellationToken);
        }

        [TestMethod]
        public async Task ScheduleAsync_ScheduleAttributeScanning()
        {
            // Arrange 
            var cancellationToken = CancellationToken.None;
            var assemblies = new[] { GetType().Assembly };

            // Act 
            await _sut.ScheduleAsync(assemblies, cancellationToken);

            // Assert
            await _scheduleProvider.Received(1).CreateAsync(Arg.Is<ScheduleItem>(x => x.Queue == queueName && x.CronExpression == cron.Expression), cancellationToken);

        }

        [Schedule("* * * * *")]
        public class ScheduleDefaultCommand : IQuidjiboCommand
        {
        }

        [Schedule("* * * * *", "other")]
        public class ScheduleDefaultAndQueueCommand : IQuidjiboCommand
        {
        }

        [Schedule("* * * * *", "other", "FancyPants")]
        public class ScheduleDefaultQueueAndNameCommand : IQuidjiboCommand
        {
        }

        [Schedule("* * * * *", "other", "FancyPants", typeof(TestClientKey1))]
        public class ScheduleDefaultQueueNameAndKeyedCommand : IQuidjiboCommand
        {
        }
    }
}