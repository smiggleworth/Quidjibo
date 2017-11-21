using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Factories;
using Quidjibo.Providers;
using Quidjibo.SqlServer.Providers;
using Quidjibo.SqlServer.Utils;

namespace Quidjibo.SqlServer.Factories
{
    public class SqlWorkProviderFactory : IWorkProviderFactory
    {
        private static readonly SemaphoreSlim SyncLock = new SemaphoreSlim(1, 1);

        private readonly int _batchSize;
        private readonly string _connectionString;
        private readonly int _visibilityTimeout;

        public SqlWorkProviderFactory(string connectionString, int visibilityTimeout = 60, int batchSize = 5)
        {
            _connectionString = connectionString;
            _visibilityTimeout = visibilityTimeout;
            _batchSize = batchSize;
        }

        public int PollingInterval => 10;

        public Task<IWorkProvider> CreateAsync(string queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CreateAsync(queues.Split(','), cancellationToken);
        }

        public async Task<IWorkProvider> CreateAsync(string[] queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await SyncLock.WaitAsync(cancellationToken);
                await SqlRunner.ExecuteAsync(async cmd =>
                {
                    var schemaSetup = await SqlLoader.GetScript("Schema.Setup");
                    var workSetup = await SqlLoader.GetScript("Work.Setup");
                    cmd.CommandText = $"{schemaSetup};\r\n{workSetup}";
                    await cmd.ExecuteNonQueryAsync(cancellationToken);
                }, _connectionString, false, cancellationToken);

                return new SqlWorkProvider(_connectionString, queues, _visibilityTimeout, _batchSize);
            }
            finally
            {
                SyncLock.Release();
            }
        }
    }
}