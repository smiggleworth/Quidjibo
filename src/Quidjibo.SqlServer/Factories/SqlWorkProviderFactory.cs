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
        private readonly string _connectionString;
        private readonly int _visibilityTimeout;

        public SqlWorkProviderFactory(string connectionString, int visibilityTimeout = 60)
        {
            _connectionString = connectionString;
            _visibilityTimeout = visibilityTimeout;
        }

        public async Task<IWorkProvider> CreateAsync(string queue, CancellationToken cancellationToken = default(CancellationToken))
        {
            var queues = queue.Split(',');

            await SqlRunner.ExecuteAsync(async cmd =>
            {
                cmd.CommandText = await SqlLoader.GetScript("Work.Setup");
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }, _connectionString, false, cancellationToken);

            return new SqlWorkProvider(_connectionString, queues, _visibilityTimeout, 5);
        }
    }
}