using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Quidjibo.Configurations;
using Quidjibo.Dispatchers;
using Quidjibo.Factories;
using Quidjibo.Models;
using Quidjibo.Providers;
using Quidjibo.Serializers;
using Quidjibo.Servers;

namespace Quidjibo.Tests.Servers
{
    [TestClass]
    public class QuidjiboServerTests
    {
        private QuidjiboServer _sut;


        private ConcurrentQueue<WorkItem> _testQueue;
        private ICronProvider _cronProvider;
        private IWorkDispatcher _dispatcher;
        private ILogger _logger;
        private ILoggerFactory _loggerFactory;
        private IProgressProviderFactory _progressProviderFactory;
        private IProgressProvider _progressProvider;
        private IQuidjiboConfiguration _quidjiboConfiguration;
        private IScheduleProviderFactory _scheduleProviderFactory;
        private IScheduleProvider _scheduleProvider;
        private IPayloadSerializer _serializer;
        private IWorkProviderFactory _workProviderFactory;
        private IWorkProvider _workProvider;

        [TestInitialize]
        public void Init()
        {
            _testQueue = new ConcurrentQueue<WorkItem>();
            GenFu.GenFu.Configure<WorkItem>().Fill(x => x.Queue, "default");
            var workItems = GenFu.GenFu.ListOf<WorkItem>();
            workItems.ForEach(x => _testQueue.Enqueue(x));

            _loggerFactory = Substitute.For<ILoggerFactory>();
            _logger = Substitute.For<ILogger<QuidjiboServer>>();
            _loggerFactory.CreateLogger<QuidjiboServer>().Returns(_logger);

            _quidjiboConfiguration = Substitute.For<IQuidjiboConfiguration>();

            _workProviderFactory = Substitute.For<IWorkProviderFactory>();
            _workProvider = Substitute.For<IWorkProvider>();
            _workProviderFactory.CreateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(_workProvider));

            _scheduleProviderFactory = Substitute.For<IScheduleProviderFactory>();
            _scheduleProvider = Substitute.For<IScheduleProvider>();
            _scheduleProviderFactory.CreateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(_scheduleProvider));

            _progressProviderFactory = Substitute.For<IProgressProviderFactory>();
            _progressProvider = Substitute.For<IProgressProvider>();
            _progressProviderFactory.CreateAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(_progressProvider));

            _dispatcher = Substitute.For<IWorkDispatcher>();
            _serializer = Substitute.For<IPayloadSerializer>();
            _cronProvider = Substitute.For<ICronProvider>();

            _sut = new QuidjiboServer(
                _loggerFactory,
                _quidjiboConfiguration,
                _workProviderFactory,
                _scheduleProviderFactory,
                _progressProviderFactory,
                _dispatcher,
                _serializer,
                _cronProvider);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _sut.Dispose();
        }

        [TestMethod]
        public void Start_SingleLoop()
        {
            // Arrange
            _quidjiboConfiguration.Throttle.Returns(1);
            _quidjiboConfiguration.LockInterval.Returns(60);
            _quidjiboConfiguration.SingleLoop.Returns(true);
            _quidjiboConfiguration.Queues.Returns(new List<string>(2) {"default", "other"});
            _workProvider.ReceiveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                         .Returns(x =>
                         {
                             WorkItem item;
                             _testQueue.TryPeek(out item);
                             return Task.FromResult(new List<WorkItem> { item });
                         });

            _workProvider.RenewAsync(Arg.Any<WorkItem>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(DateTime.UtcNow.AddMinutes(1)));
            _workProvider.CompleteAsync(Arg.Any<WorkItem>(), Arg.Any<CancellationToken>())
                         .Returns(x =>
                         {
                             WorkItem item;
                             _testQueue.TryDequeue(out item);
                             return Task.CompletedTask;
                         });

            // Act
            _sut.Start();

            // Assert
            while (_testQueue.Any())
            {

            }
            _testQueue.Count.Should().Be(0);
        }

        [TestMethod]
        public void Start_MultipleLoop() { }
    }
}
