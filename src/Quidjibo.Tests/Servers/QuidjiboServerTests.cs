﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Quidjibo.Commands;
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
        private ICronProvider _cronProvider;

        private CancellationTokenSource _cts;
        private IWorkDispatcher _dispatcher;
        private ILogger _logger;
        private ILoggerFactory _loggerFactory;
        private IProgressProvider _progressProvider;
        private IProgressProviderFactory _progressProviderFactory;
        private IQuidjiboConfiguration _quidjiboConfiguration;
        private IScheduleProvider _scheduleProvider;
        private IScheduleProviderFactory _scheduleProviderFactory;
        private IPayloadSerializer _serializer;
        private QuidjiboServer _sut;
        private IWorkProvider _workProvider;
        private IWorkProviderFactory _workProviderFactory;

        [TestInitialize]
        public void Init()
        {
            _cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            _loggerFactory = Substitute.For<ILoggerFactory>();
            _logger = Substitute.For<ILogger<QuidjiboServer>>();
            _loggerFactory.CreateLogger<QuidjiboServer>().Returns(_logger);

            _quidjiboConfiguration = Substitute.For<IQuidjiboConfiguration>();

            _workProviderFactory = Substitute.For<IWorkProviderFactory>();
            _workProvider = Substitute.For<IWorkProvider>();
            _workProviderFactory.CreateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(_workProvider));
            _workProviderFactory.PollingInterval.Returns(1);

            _scheduleProviderFactory = Substitute.For<IScheduleProviderFactory>();
            _scheduleProvider = Substitute.For<IScheduleProvider>();
            _scheduleProviderFactory.CreateAsync(Arg.Any<List<string>>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(_scheduleProvider));
            _scheduleProviderFactory.PollingInterval.Returns(1);

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

        [DataTestMethod]
        [DataRow(1, true, DisplayName = "Throttle 1 in Single Loop")]
        [DataRow(2, true, DisplayName = "Throttle 2 in Single Loop")]
        [DataRow(3, true, DisplayName = "Throttle 3 in Single Loop")]
        [DataRow(1, false, DisplayName = "Throttle 1 in Multiple Loop")]
        [DataRow(2, false, DisplayName = "Throttle 2 in Multiple Loop")]
        [DataRow(3, false, DisplayName = "Throttle 3 in Multiple Loop")]
        public void WorkLoopAsyncTest(int throttle, bool singleLoop)
        {
            // Arrange 
            var testQueue = new ConcurrentQueue<WorkItem>();
            var completedWork = new ConcurrentBag<WorkItem>();
            var scheduledItems = new List<ScheduleItem>();

            var defaultItems = GenFu.GenFu.ListOf<WorkItem>();
            defaultItems.ForEach(x =>
            {
                x.Queue = "default";
                testQueue.Enqueue(x);
            });

            var primaryItems = GenFu.GenFu.ListOf<WorkItem>();
            primaryItems.ForEach(x =>
            {
                x.Queue = "primary";
                testQueue.Enqueue(x);
            });

            var secondaryItems = GenFu.GenFu.ListOf<WorkItem>();
            secondaryItems.ForEach(x =>
            {
                x.Queue = "secondary";
                testQueue.Enqueue(x);
            });

            _quidjiboConfiguration.Throttle.Returns(throttle);
            _quidjiboConfiguration.LockInterval.Returns(60);
            _quidjiboConfiguration.SingleLoop.Returns(singleLoop);
            _quidjiboConfiguration.Queues.Returns(new List<string>(2) { "default", "primary", "secondary" });
            _workProvider.ReceiveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                         .Returns(x =>
                         {
                             WorkItem item;
                             testQueue.TryDequeue(out item);
                             return Task.FromResult(new List<WorkItem> { item });
                         });

            _workProvider.RenewAsync(Arg.Any<WorkItem>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(DateTime.UtcNow.AddMinutes(1)));
            _workProvider.CompleteAsync(Arg.Any<WorkItem>(), Arg.Any<CancellationToken>())
                         .Returns(x =>
                         {
                             completedWork.Add(x.Arg<WorkItem>());
                             return Task.CompletedTask;
                         });
            _scheduleProvider.ReceiveAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(scheduledItems));

            // Act
            _sut.Start();

            // Assert
            while (completedWork.Count() != 75 && !_cts.IsCancellationRequested)
            {
                // waiting for server to process all items
            }
            testQueue.Count.Should().Be(0);
            if (singleLoop)
            {
                _workProviderFactory.Received(1).CreateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
            }
            else
            {
                _workProviderFactory.Received(1).CreateAsync(Arg.Is<string>(x => x == "default"), Arg.Any<CancellationToken>());
                _workProviderFactory.Received(1).CreateAsync(Arg.Is<string>(x => x == "primary"), Arg.Any<CancellationToken>());
                _workProviderFactory.Received(1).CreateAsync(Arg.Is<string>(x => x == "secondary"), Arg.Any<CancellationToken>());
            }
            _dispatcher.ReceivedWithAnyArgs(75).DispatchAsync(Arg.Any<IQuidjiboCommand>(), Arg.Any<IProgress<Tracker>>(), Arg.Any<CancellationToken>());
        }

        [TestMethod]
        public void ScheduleLoopAsyncTest()
        {
            // Arrange
            var workItems = new List<WorkItem>();
            var scheduledItems = GenFu.GenFu.ListOf<ScheduleItem>();
            var completedSchedules = new ConcurrentBag<ScheduleItem>();
            _quidjiboConfiguration.Throttle.Returns(1);
            _quidjiboConfiguration.LockInterval.Returns(60);
            _quidjiboConfiguration.SingleLoop.Returns(true);
            _quidjiboConfiguration.Queues.Returns(new List<string>(2) { "default", "other" });

            _workProvider.ReceiveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(workItems));
            _scheduleProvider.ReceiveAsync(Arg.Any<CancellationToken>()).Returns(x => Task.FromResult(scheduledItems.Except(completedSchedules).Take(5).ToList()));
            _scheduleProvider.CompleteAsync(Arg.Any<ScheduleItem>(), Arg.Any<CancellationToken>()).Returns(x =>
            {
                completedSchedules.Add(x.Arg<ScheduleItem>());
                return Task.CompletedTask;
            });

            // Act
            _sut.Start();

            // Assert
            while (completedSchedules.Count < 25 && !_cts.IsCancellationRequested)
            {
                // waiting for server to process all items
            }
            completedSchedules.Should().Contain(scheduledItems);
            _workProvider.Received(25).SendAsync(Arg.Any<WorkItem>(), 1, Arg.Any<CancellationToken>());
        }
    }
}