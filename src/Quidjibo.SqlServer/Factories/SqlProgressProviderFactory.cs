using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Factories;
using Quidjibo.Providers;
using Quidjibo.SqlServer.Providers;
using Quidjibo.SqlServer.Utils;

namespace Quidjibo.SqlServer.Factories
{
    public class SqlProgressProviderFactory : IProgressProviderFactory
    {
        private static readonly SemaphoreSlim SyncLock = new SemaphoreSlim(1, 1);

        private readonly string _connectionString;
        private IProgressProvider _provider;

        public SqlProgressProviderFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IProgressProvider> CreateAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_provider != null)
            {
                return _provider;
            }
            try
            {
                await SyncLock.WaitAsync(cancellationToken);
                await SqlRunner.ExecuteAsync(async cmd =>
                {
                    cmd.CommandText = await SqlLoader.GetScript("Progress.Setup");
                    await cmd.ExecuteNonQueryAsync(cancellationToken);
                }, _connectionString, false, cancellationToken);

                _provider = new SqlProgressProvider(_connectionString);
                return _provider;
            }
            finally
            {
                SyncLock.Release();
            }
        }
    }
}