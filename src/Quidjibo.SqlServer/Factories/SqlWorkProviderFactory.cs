﻿using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Factories;
using Quidjibo.Providers;
using Quidjibo.SqlServer.Configurations;
using Quidjibo.SqlServer.Providers;
using Quidjibo.SqlServer.Utils;

namespace Quidjibo.SqlServer.Factories
{
    public class SqlWorkProviderFactory : IWorkProviderFactory
    {
        private readonly SqlServerQuidjiboConfiguration _sqlServerQuidjiboConfiguration;
        private static readonly SemaphoreSlim SyncLock = new SemaphoreSlim(1, 1);
        private bool _initialized;

        public SqlWorkProviderFactory(SqlServerQuidjiboConfiguration sqlServerQuidjiboConfiguration)
        {
            _sqlServerQuidjiboConfiguration = sqlServerQuidjiboConfiguration;
        }

        public Task<IWorkProvider> CreateAsync(string queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CreateAsync(queues.Split(','), cancellationToken);
        }

        public async Task<IWorkProvider> CreateAsync(string[] queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await SyncLock.WaitAsync(cancellationToken);
                if (!_initialized)
                {
                    await SqlRunner.ExecuteAsync(async cmd =>
                    {
                        var schemaSetup = await SqlLoader.GetScript("Schema.Setup");
                        var workSetup = await SqlLoader.GetScript("Work.Setup");
                        cmd.CommandText = $"{schemaSetup};\r\n{workSetup}";
                        await cmd.ExecuteNonQueryAsync(cancellationToken);
                    }, _sqlServerQuidjiboConfiguration.ConnectionString, false, cancellationToken);
                    _initialized = true;
                }
                return new SqlWorkProvider(
                    _sqlServerQuidjiboConfiguration.ConnectionString, 
                    _sqlServerQuidjiboConfiguration.Queues, 
                    _sqlServerQuidjiboConfiguration.LockInterval,
                    _sqlServerQuidjiboConfiguration.BatchSize);
            }
            finally
            {
                SyncLock.Release();
            }
        }
    }
}