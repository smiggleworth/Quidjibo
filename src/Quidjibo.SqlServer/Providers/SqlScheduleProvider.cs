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
    public class SqlScheduleProvider : IScheduleProvider
    {
        private readonly string _connectionString;
        private readonly string[] _queues;
        private string _completeSql;
        private string _createSql;
        private string _existsSql;
        private string _receiveSql;

        public SqlScheduleProvider(string connectionString, string[] queues)
        {
            _connectionString = connectionString;
            _queues = queues;
        }

        public async Task<List<ScheduleItem>> ReceiveAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var receiveOn = DateTime.UtcNow;

            if (_receiveSql == null)
            {
                _receiveSql = await SqlLoader.GetScript("Schedule.Receive");
                if (_queues.Length > 0)
                {
                    _receiveSql = _receiveSql.Replace("@Queue1",
                        string.Join(",", _queues.Select((x, i) => $"@Queue{i}")));
                }
            }
            var items = new List<ScheduleItem>(100);
            await ExecuteAsync(async cmd =>
            {
                cmd.CommandText = _receiveSql;
                cmd.AddParameter("@Take", 100);
                cmd.AddParameter("@ReceiveOn", receiveOn);
                cmd.AddParameter("@VisibleOn", receiveOn.AddSeconds(60));

                // dynamic parameters
                _queues.Select((q, i) => cmd.Parameters.AddWithValue($"@Queue{i}", q)).ToList();
                using (var rdr = await cmd.ExecuteReaderAsync(cancellationToken))
                {
                    while (await rdr.ReadAsync(cancellationToken))
                    {
                        var item = MapScheduleItem(rdr);
                        items.Add(item);
                    }
                }
            }, cancellationToken);
            return items;
        }

        public async Task CompleteAsync(ScheduleItem item, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_completeSql == null)
            {
                _completeSql = await SqlLoader.GetScript("Schedule.Complete");
            }
            await ExecuteAsync(async cmd =>
            {
                cmd.CommandText = _completeSql;
                cmd.AddParameter("@Id", item.Id);
                cmd.AddParameter("@EnqueueOn", item.EnqueueOn);
                cmd.AddParameter("@EnqueuedOn", item.EnqueuedOn);
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }, cancellationToken);
        }

        public async Task CreateAsync(ScheduleItem item, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_createSql == null)
            {
                _createSql = await SqlLoader.GetScript("Schedule.Create");
            }
            await ExecuteAsync(async cmd =>
            {
                cmd.CommandText = _createSql;
                cmd.AddParameter("@Id", item.Id);
                cmd.AddParameter("@Name", item.Name);
                cmd.AddParameter("@Queue", item.Queue);
                cmd.AddParameter("@CronExpression", item.CronExpression);
                cmd.AddParameter("@CreatedOn", item.CreatedOn);
                cmd.AddParameter("@EnqueuedOn", item.EnqueuedOn);
                cmd.AddParameter("@EnqueueOn", item.EnqueueOn);
                cmd.AddParameter("@VisibleOn", item.VisibleOn);
                cmd.AddParameter("@Payload", item.Payload);
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }, cancellationToken);
        }

        public async Task<ScheduleItem> LoadByNameAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            var item = default(ScheduleItem);
            await ExecuteAsync(async cmd =>
            {
                cmd.CommandText = await SqlLoader.GetScript("Schedule.LoadByName");
                cmd.AddParameter("@Name", name);
                using (var rdr = await cmd.ExecuteReaderAsync(cancellationToken))
                {
                    if (await rdr.ReadAsync(cancellationToken))
                    {
                        item = MapScheduleItem(rdr);
                    }
                }
            }, cancellationToken);
            return item;
        }

        public Task UpdateAsync(ScheduleItem item, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            await ExecuteAsync(async cmd =>
            {
                cmd.CommandText = await SqlLoader.GetScript("Schedule.Delete");
                cmd.AddParameter("@Id", id);
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }, cancellationToken);
        }

        public async Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_existsSql == null)
            {
                _existsSql = await SqlLoader.GetScript("Schedule.Exists");
            }
            var count = 0;
            await ExecuteAsync(async cmd =>
            {
                cmd.CommandText = _existsSql;
                cmd.AddParameter("@Name", name);
                count = (int)await cmd.ExecuteScalarAsync(cancellationToken);
            }, cancellationToken);
            return count > 0;
        }

        private ScheduleItem MapScheduleItem(SqlDataReader rdr)
        {
            return new ScheduleItem
            {
                CreatedOn = rdr.Map<DateTime>(nameof(ScheduleItem.CreatedOn)),
                CronExpression = rdr.Map<string>(nameof(ScheduleItem.CronExpression)),
                EnqueuedOn = rdr.Map<DateTime?>(nameof(ScheduleItem.EnqueuedOn)),
                EnqueueOn = rdr.Map<DateTime?>(nameof(ScheduleItem.EnqueueOn)),
                Id = rdr.Map<Guid>(nameof(ScheduleItem.Id)),
                Name = rdr.Map<string>(nameof(ScheduleItem.Name)),
                Payload = rdr.Map<byte[]>(nameof(ScheduleItem.Payload)),
                Queue = rdr.Map<string>(nameof(ScheduleItem.Queue))
            };
        }

        private Task ExecuteAsync(Func<SqlCommand, Task> func, CancellationToken cancellationToken)
        {
            return SqlRunner.ExecuteAsync(func, _connectionString, true, cancellationToken);
        }
    }
}