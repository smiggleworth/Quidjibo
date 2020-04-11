using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GenFu;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Quidjibo.Attributes;
using Quidjibo.Clients;
using Quidjibo.Commands;
using Quidjibo.Constants;
using Quidjibo.Factories;
using Quidjibo.Models;
using Quidjibo.Protectors;
using Quidjibo.Providers;
using Quidjibo.Serializers;
using Quidjibo.Tests.Samples;

namespace Quidjibo.Tests.Clients
{
    [TestClass]
    public class QuidjiboClientTests
    {
        private ICronProvider _cronProvider;

        private ILoggerFactory _loggerFactory;
        private IPayloadProtector _payloadProtector;
        private IPayloadSerializer _payloadSerializer;
        private IScheduleProvider _scheduleProvider;
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
            _payloadProtector = Substitute.For<IPayloadProtector>();
            _cronProvider = Substitute.For<ICronProvider>();

            _sut = new QuidjiboClient(
                _loggerFactory,
                _workProviderFactory,
                _scheduleProviderFactory,
                _payloadSerializer,
                _payloadProtector,
                _cronProvider);
            _sut.Clear();
        }

        [TestMethod]
        public async Task PublishAsync()
        {
            // Arrange
            var queueName = Default.Queue;
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
            var queueName = Default.Queue;
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
        public async Task PublishAsync_WithDelayAndExpireOnAndQueueName()
        {
            // Arrange
            var queueName = BaseValueGenerator.Word();
            var command = new BasicCommand();
            var delay = GenFu.GenFu.Random.Next();
            var expireOn = new DateTime(2032, 2, 27);
            var cancellationToken = CancellationToken.None;
            _payloadSerializer.SerializeAsync(command, cancellationToken).Returns(Task.FromResult<byte[]>(null));
            _workProviderFactory.CreateAsync(queueName, cancellationToken).Returns(Task.FromResult(_workProvider));

            // Act 
            await _sut.PublishAsync(command, queueName, delay, expireOn, cancellationToken);

            // Assert
            await _workProvider.Received(1).SendAsync(Arg.Is<WorkItem>(x => x.Queue == queueName && x.ExpireOn == expireOn), delay, cancellationToken);
        }

        [TestMethod]
        public async Task ScheduleAsync()
        {
            // Arrange
            var name = BaseValueGenerator.Word();
            var queueName = Default.Queue;
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
            var queueName = Default.Queue;
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
            _payloadProtector.ProtectAsync(payload, cancellationToken).Returns(Task.FromResult(payload));
            _scheduleProviderFactory.CreateAsync(queueName, cancellationToken).Returns(Task.FromResult(_scheduleProvider));
            _scheduleProvider.LoadByNameAsync(name, cancellationToken).Returns(Task.FromResult(existingItem));

            // Act 
            await _sut.ScheduleAsync(name, command, cron, cancellationToken);

            // Assert
            await _scheduleProvider.DidNotReceiveWithAnyArgs().CreateAsync(default(ScheduleItem), cancellationToken);
        }

        [TestMethod]
        public async Task ScheduleAsync_DeleteSchedulesThatExistButHaveDifferentCronExpressions()
        {
            // Arrange
            var name = BaseValueGenerator.Word();
            var queueName = Default.Queue;
            var cron = Cron.Daily();
            var payload = Guid.NewGuid().ToByteArray();
            var command = new BasicCommand();
            var cancellationToken = CancellationToken.None;
            var existingItem = new ScheduleItem
            {
                Id = Guid.NewGuid(),
                CronExpression = Cron.Weekly().Expression,
                Name = name,
                Payload = Guid.NewGuid().ToByteArray(),
                Queue = queueName
            };

            _payloadSerializer.SerializeAsync(command, cancellationToken).Returns(Task.FromResult(payload));
            _scheduleProviderFactory.CreateAsync(queueName, cancellationToken).Returns(Task.FromResult(_scheduleProvider));
            _scheduleProvider.LoadByNameAsync(name, cancellationToken).Returns(Task.FromResult(existingItem));

            // Act 
            await _sut.ScheduleAsync(name, command, Cron.Daily(), cancellationToken);

            // Assert
            await _scheduleProvider.Received(1).DeleteAsync(existingItem.Id, cancellationToken);
            await _scheduleProvider.Received(1).CreateAsync(Arg.Is<ScheduleItem>(x => x.Queue == queueName && x.CronExpression == cron.Expression), cancellationToken);
        }

        [TestMethod]
        public async Task ScheduleAsync_ScheduleAttributeScanningDefaultClientKey()
        {
            // Arrange 
            var cancellationToken = CancellationToken.None;
            var assemblies = new[] {GetType().Assembly};
            _scheduleProviderFactory.CreateAsync(Arg.Any<string>(), cancellationToken).Returns(Task.FromResult(_scheduleProvider));

            // Act 
            await _sut.ScheduleAsync(assemblies, cancellationToken);

            // Assert
            await _scheduleProvider.Received(1).CreateAsync(Arg.Is<ScheduleItem>(x => x.Queue == Default.Queue && x.CronExpression == "* * * * *"), cancellationToken);
            await _scheduleProvider.Received(1).CreateAsync(Arg.Is<ScheduleItem>(x => x.Queue == Default.Queue && x.Name == "MinuteIntervalsScheduleDefaultCommand"), cancellationToken);
            await _scheduleProvider.Received(1).CreateAsync(Arg.Is<ScheduleItem>(x => x.Queue == Default.Queue && x.Name == "DailyScheduleDefaultCommand"), cancellationToken);
            await _scheduleProvider.Received(1).CreateAsync(Arg.Is<ScheduleItem>(x => x.Queue == Default.Queue && x.Name == "WeeklyScheduleDefaultCommand"), cancellationToken);

            await _scheduleProvider.Received(1).CreateAsync(Arg.Is<ScheduleItem>(x => x.Queue == "queue-1" && x.CronExpression == "1 * * * *"), cancellationToken);
            await _scheduleProvider.Received(1).CreateAsync(Arg.Is<ScheduleItem>(x => x.Queue == "queue-1" && x.Name == "MinuteIntervalsScheduleDefaultCommand"), cancellationToken);
            await _scheduleProvider.Received(1).CreateAsync(Arg.Is<ScheduleItem>(x => x.Queue == "queue-1" && x.Name == "DailyScheduleDefaultCommand"), cancellationToken);
            await _scheduleProvider.Received(1).CreateAsync(Arg.Is<ScheduleItem>(x => x.Queue == "queue-1" && x.Name == "WeeklyScheduleDefaultCommand"), cancellationToken);
        }

        [TestMethod]
        public async Task ScheduleAsync_ShouldHandleNoAssemblies()
        {
            // Arrange 
            var cancellationToken = CancellationToken.None;
            _scheduleProviderFactory.CreateAsync(Arg.Any<string>(), cancellationToken).Returns(Task.FromResult(_scheduleProvider));

            // Act 
            await _sut.ScheduleAsync(new Assembly[0], cancellationToken);

            // Assert
            await _scheduleProvider.DidNotReceiveWithAnyArgs().CreateAsync(Arg.Any<ScheduleItem>(), cancellationToken);
        }

        [TestMethod]
        public async Task ScheduleAsync_ShouldHandleNullAssemblies()
        {
            // Arrange 
            var cancellationToken = CancellationToken.None;
            _scheduleProviderFactory.CreateAsync(Arg.Any<string>(), cancellationToken).Returns(Task.FromResult(_scheduleProvider));

            // Act 
            await _sut.ScheduleAsync(null, cancellationToken);

            // Assert
            await _scheduleProvider.DidNotReceiveWithAnyArgs().CreateAsync(Arg.Any<ScheduleItem>(), cancellationToken);
        }

        [MinuteIntervalsSchedule("MinuteIntervalsScheduleDefaultCommand", 7)]
        [DailySchedule("DailyScheduleDefaultCommand", 1, 0)]
        [WeeklySchedule("WeeklyScheduleDefaultCommand", DayOfWeek.Friday, 1, 0)]
        [Schedule(nameof(ScheduleDefaultCommand), "* * * * *")]
        public class ScheduleDefaultCommand : IQuidjiboCommand
        {
            public Guid? CorrelationId { get; set; }
            public Dictionary<string, string> Metadata { get; set; }
        }

        [MinuteIntervalsSchedule("MinuteIntervalsScheduleDefaultCommand", 7, "queue-1")]
        [DailySchedule("DailyScheduleDefaultCommand", 1, 0, "queue-1")]
        [WeeklySchedule("WeeklyScheduleDefaultCommand", DayOfWeek.Friday, 1, 0, "queue-1")]
        [Schedule(nameof(SchedulCustomQueueCommand), "1 * * * *", "queue-1")]
        public class SchedulCustomQueueCommand : IQuidjiboCommand
        {
            public Guid? CorrelationId { get; set; }
            public Dictionary<string, string> Metadata { get; set; }
        }

        [Schedule(nameof(ScheduleCustomeQueueAndKeyedCommand), "1 1 * * *", "queue-2", typeof(CustomClientKey1))]
        public class ScheduleCustomeQueueAndKeyedCommand : IQuidjiboCommand
        {
            public Guid? CorrelationId { get; set; }
            public Dictionary<string, string> Metadata { get; set; }
        }
    }
}