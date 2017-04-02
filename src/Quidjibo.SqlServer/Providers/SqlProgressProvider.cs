using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Models;
using Quidjibo.Providers;
using Quidjibo.SqlServer.Extensions;
using Quidjibo.SqlServer.Utils;

namespace Quidjibo.SqlServer.Providers
{
    public class SqlProgressProvider : IProgressProvider
    {
        private readonly string _connectionString;
        private string _reportSql;

        public SqlProgressProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task ReportAsync(ProgressItem item, CancellationToken cancellationToken)
        {
            if (_reportSql == null)
            {
                _reportSql = await SqlLoader.GetScript("Progress.Create");
            }
            await ExecuteAsync(async cmd =>
            {
                cmd.CommandText = _reportSql;
                cmd.AddParameter("@Id", item.Id);
                cmd.AddParameter("@WorkId", item.WorkId);
                cmd.AddParameter("@CorrelationId", item.CorrelationId);
                cmd.AddParameter("@Name", item.Name);
                cmd.AddParameter("@Queue", item.Queue);
                cmd.AddParameter("@RecordedOn", item.RecordedOn);
                cmd.AddParameter("@Value", item.Value);
                cmd.AddParameter("@Note", item.Note);
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }, cancellationToken);
            await Task.CompletedTask;
        }

        private Task ExecuteAsync(Func<SqlCommand, Task> func, CancellationToken cancellationToken)
        {
            return SqlRunner.ExecuteAsync(func, _connectionString, true, cancellationToken);
        }
    }
}