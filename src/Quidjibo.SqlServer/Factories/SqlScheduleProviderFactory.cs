using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Factories;
using Quidjibo.Providers;
using Quidjibo.SqlServer.Providers;
using Quidjibo.SqlServer.Utils;

namespace Quidjibo.SqlServer.Factories
{
    public class SqlScheduleProviderFactory : IScheduleProviderFactory
    {
        private readonly string _connectionString;

        public SqlScheduleProviderFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IScheduleProvider> CreateAsync(List<string> queues,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await SqlRunner.ExecuteAsync(async cmd =>
            {
                cmd.CommandText = await SqlLoader.GetScript("Schedule.Setup");
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }, _connectionString, false, cancellationToken);


            return await Task.FromResult<IScheduleProvider>(new SqlScheduleProvider(_connectionString, queues));
        }

        public async Task<IScheduleProvider> CreateAsync(string queue,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await CreateAsync(new List<string>
            {
                queue
            }, cancellationToken);
        }
    }
}