using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quidjibo.Commands;
using Quidjibo.Configurations;
using Quidjibo.Factories;
using Quidjibo.Misc;
using Quidjibo.Models;
using Quidjibo.Pipeline;
using Quidjibo.Pipeline.Contexts;
using Quidjibo.Pipeline.Misc;
using Quidjibo.Providers;

namespace Quidjibo.Servers
{
    public class QuidjiboServer : IQuidjiboServer
    {
        private readonly IQuidjiboPipeline _quidjiboPipeline;
        private readonly ICronProvider _cronProvider;
        private readonly ILogger _logger;
        private readonly IProgressProviderFactory _progressProviderFactory;
        private readonly IQuidjiboConfiguration _quidjiboConfiguration;
        private readonly IScheduleProviderFactory _scheduleProviderFactory;
        private readonly IWorkProviderFactory _workProviderFactory;

        public string Worker { get; }

        public bool IsRunning { get; private set; }

        public QuidjiboServer(
            ILoggerFactory loggerFactory,
            IQuidjiboConfiguration quidjiboConfiguration,
            IWorkProviderFactory workProviderFactory,
            IScheduleProviderFactory scheduleProviderFactory,
            IProgressProviderFactory progressProviderFactory,
            ICronProvider cronProvider, IQuidjiboPipeline quidjiboPipeline)
        {
            _logger = loggerFactory.CreateLogger<QuidjiboServer>();
            _workProviderFactory = workProviderFactory;
            _scheduleProviderFactory = scheduleProviderFactory;
            _quidjiboConfiguration = quidjiboConfiguration;
            _cronProvider = cronProvider;
            _quidjiboPipeline = quidjiboPipeline;
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
                _logger.LogInformation("Starting Server {0}", Worker);
                _cts = new CancellationTokenSource();
                _loopTasks = new List<Task>();

                _throttle = new SemaphoreSlim(0, _quidjiboConfiguration.Throttle);
                if (_quidjiboConfiguration.EnableWorker)
                {
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
                }

                if (_quidjiboConfiguration.EnableScheduler)
                {
                    _logger.LogInformation("Enabling scheduler");
                    _loopTasks.Add(ScheduleLoopAsync(_quidjiboConfiguration.Queues));
                }
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
                        var tasks = items.Select(item => InvokePipelineAsync(workProvider, item));
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

        private async Task InvokePipelineAsync(IWorkProvider provider, WorkItem item)
        {
            using (var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token))
            {
                var progress = new QuidjiboProgress();
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

                var renewTask = RenewAsync(provider, item, linkedTokenSource.Token);
                try
                {
                    var context = new QuidjiboContext
                    {
                        Item = item,
                        Provider = provider,
                        Progress = progress,
                        State = new PipelineState()
                    };
                    await _quidjiboPipeline.StartAsync(context, linkedTokenSource.Token);

                    if (context.State.Success)
                    {
                        await provider.CompleteAsync(item, linkedTokenSource.Token);
                        _logger.LogDebug("Completed : {0}", item.Id);
                    }
                    else
                    {
                        await provider.FaultAsync(item, linkedTokenSource.Token);
                        _logger.LogError(null, "Faulted : {0}", item.Id);
                    }
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