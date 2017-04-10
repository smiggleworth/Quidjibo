using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Models;
using Quidjibo.Providers;
using Quidjibo.SqlServer.Extensions;
using Quidjibo.SqlServer.Utils;

namespace Quidjibo.SqlServer.Providers
{
    public class SqlWorkProvider : IWorkProvider
    {
        public enum StatusFlags
        {
            Faulted = -1,
            New = 0,
            InFlight = 1,
            Complete = 2
        }

        private readonly int _batchSize;
        private readonly string _connectionString;
        private readonly int _maxAttempts;
        private readonly string[] _queues;
        private readonly int _visibilityTimeout;
        private string _completeSql;
        private string _faultSql;
        private string _receiveSql;
        private string _renewSql;
        private string _sendSql;

        public async Task SendAsync(WorkItem item, int delay, CancellationToken cancellationToken)
        {
            if (_sendSql == null)
            {
                _sendSql = await SqlLoader.GetScript("Work.Send");
            }
            var createdOn = DateTime.UtcNow;
            await ExecuteAsync(async cmd =>
            {
                cmd.CommandText = _sendSql;
                cmd.AddParameter("@Id", item.Id);
                cmd.AddParameter("@ScheduleId", item.ScheduleId);
                cmd.AddParameter("@CorrelationId", item.CorrelationId);
                cmd.AddParameter("@Name", item.Name);
                cmd.AddParameter("@Worker", item.Worker);
                cmd.AddParameter("@Queue", item.Queue);
                cmd.AddParameter("@Attempts", item.Attempts);
                cmd.AddParameter("@CreatedOn", createdOn);
                cmd.AddParameter("@ExpireOn", createdOn.AddDays(4));
                cmd.AddParameter("@VisibleOn", createdOn.AddSeconds(delay));
                cmd.AddParameter("@Status", StatusFlags.New);
                cmd.AddParameter("@Payload", item.Payload);
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }, cancellationToken);
        }

        public async Task<List<WorkItem>> ReceiveAsync(string worker, CancellationToken cancellationToken)
        {
            var receiveOn = DateTime.UtcNow;
            if (_receiveSql == null)
            {
                _receiveSql = await SqlLoader.GetScript("Work.Receive");
                if (_queues.Length > 1)
                {
                    _receiveSql = _receiveSql.Replace("@Queue1",
                        string.Join(",", _queues.Select((x, i) => $"@Queue{i}")));
                }
            }
            var workItems = new List<WorkItem>(_batchSize);
            await ExecuteAsync(async cmd =>
            {
                cmd.CommandText = _receiveSql;
                cmd.AddParameter("@Worker", worker);
                cmd.AddParameter("@Take", _batchSize);
                cmd.AddParameter("@InFlight", StatusFlags.InFlight);
                cmd.AddParameter("@VisibleOn", receiveOn.AddSeconds(Math.Max(_visibilityTimeout, 30)));
                cmd.AddParameter("@ReceiveOn", receiveOn);
                cmd.AddParameter("@MaxAttempts", _maxAttempts);

                // dynamic parameters
                _queues.Select((q, i) => new
                       {
                           q,
                           i
                       })
                       .ToList()
                       .ForEach(x => cmd.Parameters.AddWithValue($"@Queue{x.i}", x.q));
                using (var rdr = await cmd.ExecuteReaderAsync(cancellationToken))
                {
                    while (await rdr.ReadAsync(cancellationToken))
                    {
                        var workItem = new WorkItem
                        {
                            Attempts = rdr.Map<int>(nameof(WorkItem.Attempts)),
                            CorrelationId = rdr.Map<Guid>(nameof(WorkItem.CorrelationId)),
                            ExpireOn = rdr.Map<DateTime>(nameof(WorkItem.ExpireOn)),
                            Id = rdr.Map<Guid>(nameof(WorkItem.Id)),
                            Name = rdr.Map<string>(nameof(WorkItem.Name)),
                            Payload = rdr.Map<byte[]>(nameof(WorkItem.Payload)),
                            Queue = rdr.Map<string>(nameof(WorkItem.Queue)),
                            ScheduleId = rdr.Map<Guid?>(nameof(WorkItem.ScheduleId))
                        };
                        workItem.Token = workItem.Id.ToString();
                        workItems.Add(workItem);
                    }
                }
            }, cancellationToken);
            return workItems;
        }

        public async Task<DateTime> RenewAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            if (_renewSql == null)
            {
                _renewSql = await SqlLoader.GetScript("Work.Renew");
            }
            var lockExpireOn = (workItem.VisibleOn ?? DateTime.UtcNow).AddSeconds(Math.Max(_visibilityTimeout, 30));
            await ExecuteAsync(async cmd =>
            {
                cmd.AddParameter("@Id", workItem.Id);
                cmd.AddParameter("@VisibleOn", lockExpireOn);
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }, cancellationToken);
            return lockExpireOn;
        }

        public async Task CompleteAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            if (_completeSql == null)
            {
                _completeSql = await SqlLoader.GetScript("Work.Complete");
            }
            await ExecuteAsync(async cmd =>
            {
                cmd.CommandText = _completeSql;
                cmd.AddParameter("@Id", workItem.Id);
                cmd.AddParameter("@Complete", StatusFlags.Complete);
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }, cancellationToken);
        }

        public async Task FaultAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            var faultedOn = DateTime.UtcNow;
            if (_faultSql == null)
            {
                _faultSql = await SqlLoader.GetScript("Work.Fault");
            }
            await ExecuteAsync(async cmd =>
            {
                cmd.CommandText = _faultSql;
                cmd.AddParameter("@Id", workItem.Id);
                cmd.AddParameter("@VisibleOn", faultedOn.AddSeconds(Math.Max(_visibilityTimeout, 30)));
                cmd.AddParameter("@Faulted", StatusFlags.Faulted);
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }, cancellationToken);
        }

        public SqlWorkProvider(string connectionString, string[] queues, int visibilityTimeout, int batchSize)
        {
            _queues = queues;
            _visibilityTimeout = visibilityTimeout;
            _batchSize = batchSize;
            _maxAttempts = 10;
            _connectionString = connectionString;
        }

        private Task ExecuteAsync(Func<SqlCommand, Task> func, CancellationToken cancellationToken)
        {
            return SqlRunner.ExecuteAsync(func, _connectionString, true, cancellationToken);
        }
    }
}