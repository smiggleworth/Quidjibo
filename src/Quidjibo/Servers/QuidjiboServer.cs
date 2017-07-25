using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quidjibo.Commands;
using Quidjibo.Configurations;
using Quidjibo.Dispatchers;
using Quidjibo.Factories;
using Quidjibo.Models;
using Quidjibo.Providers;
using Quidjibo.Serializers;

namespace Quidjibo.Servers
{
    public class QuidjiboServer : IQuidjiboServer
    {
        private readonly ICronProvider _cronProvider;
        private readonly IWorkDispatcher _dispatcher;
        private readonly ILogger _logger;
        private readonly IProgressProviderFactory _progressProviderFactory;
        private readonly IQuidjiboConfiguration _quidjiboConfiguration;
        private readonly IScheduleProviderFactory _scheduleProviderFactory;
        private readonly IPayloadSerializer _serializer;
        private readonly IWorkProviderFactory _workProviderFactory;
        public string Worker { get; }

        public bool IsRunning { get; private set; }

        public QuidjiboServer(
            ILoggerFactory loggerFactory,
            IQuidjiboConfiguration quidjiboConfiguration,
            IWorkProviderFactory workProviderFactory,
            IScheduleProviderFactory scheduleProviderFactory,
            IProgressProviderFactory progressProviderFactory,
            IWorkDispatcher dispatcher,
            IPayloadSerializer serializer,
            ICronProvider cronProvider)
        {
            _logger = loggerFactory.CreateLogger<QuidjiboServer>();
            _dispatcher = dispatcher;
            _workProviderFactory = workProviderFactory;
            _scheduleProviderFactory = scheduleProviderFactory;
            _quidjiboConfiguration = quidjiboConfiguration;
            _serializer = serializer;
            _cronProvider = cronProvider;
            _progressProviderFactory = progressProviderFactory;
            Worker = $"{Environment.GetEnvironmentVariable("COMPUTERNAME")}-{Guid.NewGuid()}";
        }

        public void Start()
        {
            lock (_syncRoot)
            {
                if (IsRunning)
                {
                    return;
                }
                _logger.LogInformation("Starting Worker {0}", Worker);
                _cts = new CancellationTokenSource();
                _loopTasks = new List<Task>();

                _throttle = new SemaphoreSlim(0, _quidjiboConfiguration.Throttle);
                if (_quidjiboConfiguration.SingleLoop)
                {
                    _logger.LogInformation("All queues can share the same loop");
                    var queues = string.Join(",", _quidjiboConfiguration.Queues);
                    _loopTasks.Add(WorkLoopAsync(queues));
                }
                else
                {
                    _logger.LogInformation("Each queue will need a designated loop");
                    _loopTasks.AddRange(_quidjiboConfiguration.Queues.Select(WorkLoopAsync));
                }

                _logger.LogInformation("Enabling scheduler");
                _loopTasks.Add(ScheduleLoopAsync(_quidjiboConfiguration.Queues));

                _throttle.Release(_quidjiboConfiguration.Throttle);
                IsRunning = true;
                _logger.LogInformation("Started Worker {0}", Worker);
            }
        }

        public void Stop()
        {
            lock (_syncRoot)
            {
                if (!IsRunning)
                {
                    return;
                }
                _cts?.Cancel();
                _cts?.Dispose();
                _loopTasks = null;
                IsRunning = false;
                _logger.LogInformation("Stopped Worker {0}", Worker);
            }
        }

        public void Dispose()
        {
            Stop();
        }

        #region Internals

        private readonly object _syncRoot = new object();

        private List<Task> _loopTasks;
        private CancellationTokenSource _cts;
        private SemaphoreSlim _throttle;

        private async Task WorkLoopAsync(string queue)
        {
            var pollingInterval = TimeSpan.FromSeconds(_workProviderFactory.PollingInterval);
            var workProvider = await _workProviderFactory.CreateAsync(queue, _cts.Token);
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    _logger.LogDebug("Throttle Count : {0}", _throttle.CurrentCount);

                    // throttle is important when there is more than one listener
                    await _throttle.WaitAsync(_cts.Token);
                    var items = await workProvider.ReceiveAsync(Worker, _cts.Token);
                    if (items.Any())
                    {
                        var tasks = items.Select(item => DispatchAsync(workProvider, item));
                        await Task.WhenAll(tasks);
                        _throttle.Release();
                        continue;
                    }
                    _throttle.Release();
                    await Task.Delay(pollingInterval, _cts.Token);
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(0, exception, exception.Message);
                }
            }
        }

        private async Task ScheduleLoopAsync(List<string> queues)
        {
            var pollingInterval = TimeSpan.FromSeconds(_scheduleProviderFactory.PollingInterval);
            var scheduleProvider = await _scheduleProviderFactory.CreateAsync(queues, _cts.Token);
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    var items = await scheduleProvider.ReceiveAsync(_cts.Token);
                    foreach (var item in items)
                    {
                        var work = new WorkItem
                        {
                            Attempts = 0,
                            CorrelationId = Guid.NewGuid(),
                            Id = Guid.NewGuid(),
                            Name = item.Name,
                            Payload = item.Payload,
                            Queue = item.Queue,
                            ScheduleId = item.Id
                        };
                        _logger.LogDebug("Enqueue the scheduled item : {0}", item.Id);
                        var workProvider = await _workProviderFactory.CreateAsync(work.Queue, _cts.Token);
                        await workProvider.SendAsync(work, 1, _cts.Token);
                        _logger.LogDebug("Update the schedule for the next run : {0}", item.Id);
                        item.EnqueueOn = _cronProvider.GetNextSchedule(item.CronExpression);
                        item.EnqueuedOn = DateTime.UtcNow;
                        await scheduleProvider.CompleteAsync(item, _cts.Token);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(0, exception, exception.Message);
                }
                finally
                {
                    await Task.Delay(pollingInterval, _cts.Token);
                }
            }
        }

        private async Task DispatchAsync(IWorkProvider provider, WorkItem item)
        {
            var progress = new ProgressTracker(item);
            progress.ProgressChanged += async (sender, tracker) =>
            {
                var progressProvider = await _progressProviderFactory.CreateAsync(_cts.Token);
                var progressItem = new ProgressItem
                {
                    Id = Guid.NewGuid(),
                    CorrelationId = item.CorrelationId,
                    RecordedOn = DateTime.UtcNow,
                    Name = item.Name,
                    Queue = item.Queue,
                    Note = tracker.Text,
                    Value = tracker.Value,
                    WorkId = item.Id
                };
                await progressProvider.ReportAsync(progressItem, _cts.Token);
            };
            using (var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token))
            {
                var renewTask = RenewAsync(provider, item, linkedTokenSource.Token);
                try
                {
                    var workCommand = await _serializer.DeserializeAsync(item.Payload, linkedTokenSource.Token);
                    var workflow = workCommand as WorkflowCommand;
                    if (workflow != null)
                    {
                        await DispatchWorkflowAsync(provider, item, workflow, progress, linkedTokenSource.Token);
                    }
                    else
                    {
                        await _dispatcher.DispatchAsync(workCommand, progress, linkedTokenSource.Token);
                    }
                    await provider.CompleteAsync(item, linkedTokenSource.Token);
                    _logger.LogDebug("Completed : {0}", item.Id);
                }
                catch (Exception exception)
                {
                    await provider.FaultAsync(item, linkedTokenSource.Token);
                    _logger.LogError(null, exception, "Faulted : {0}", item.Id);
                }
                finally
                {
                    _logger.LogDebug("Release : {0}", item.Id);
                    linkedTokenSource.Cancel();
                    await renewTask;
                }
            }
        }

        private async Task DispatchWorkflowAsync(
            IWorkProvider provider,
            WorkItem item,
            WorkflowCommand workflowCommand,
            IProgress<Tracker> progress,
            CancellationToken cancellationToken)
        {
            var tasks = workflowCommand.Entries.Where(e => e.Key == workflowCommand.CurrentStep)
                                       .SelectMany(e => e.Value, (e, c) => _dispatcher.DispatchAsync(c, progress, cancellationToken))
                                       .ToList();
            await Task.WhenAll(tasks);
            workflowCommand.NextStep();
            if (workflowCommand.CurrentStep < workflowCommand.Step)
            {
                var payload = await _serializer.SerializeAsync(workflowCommand, cancellationToken);
                var next = new WorkItem
                {
                    Id = Guid.NewGuid(),
                    CorrelationId = item.CorrelationId,
                    Attempts = 0,
                    Payload = payload,
                    Queue = item.Queue
                };
                _logger.LogDebug("Enqueue the next workflow step : {0}", item.Id);
                await provider.SendAsync(next, 0, cancellationToken);
            }
        }

        private async Task RenewAsync(IWorkProvider provider, WorkItem item, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(_quidjiboConfiguration.LockInterval), cancellationToken);
                    await provider.RenewAsync(item, cancellationToken);
                    _logger.LogDebug("Renewed : {0}", item.Id);
                }
                catch (OperationCanceledException)
                {
                    // ignore OperationCanceledExceptions
                }
            }
        }

        #endregion
    }
}