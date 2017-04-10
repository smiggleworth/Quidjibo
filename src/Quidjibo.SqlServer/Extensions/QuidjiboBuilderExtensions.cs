using Quidjibo.SqlServer.Factories;

namespace Quidjibo.SqlServer.Extensions
{
    public static class QuidjiboBuilderExtensions
    {
        /// <summary>
        ///     Use Sql Server for Work, Progress, and Scheduled Jobs
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static QuidjiboBuilder UseSqlServer(this QuidjiboBuilder builder, string connectionString)
        {
            return builder.ConfigureWorkProviderFactory(new SqlWorkProviderFactory(connectionString))
                          .ConfigureProgressProviderFactory(new SqlProgressProviderFactory(connectionString))
                          .ConfigureScheduleProviderFactory(new SqlScheduleProviderFactory(connectionString));
        }

        /// <summary>
        ///     Use Sql Server For Work
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static QuidjiboBuilder UseSqlServerForWork(this QuidjiboBuilder builder, string connectionString)
        {
            return builder.ConfigureWorkProviderFactory(new SqlWorkProviderFactory(connectionString));
        }

        /// <summary>
        ///     Use Sql Server For Progress
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static QuidjiboBuilder UseSqlServerForProgress(this QuidjiboBuilder builder, string connectionString)
        {
            return builder.ConfigureProgressProviderFactory(new SqlProgressProviderFactory(connectionString));
        }

        /// <summary>
        ///     Use Sql Server For Scheduled Jobs
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static QuidjiboBuilder UseSqlServerForSchedule(this QuidjiboBuilder builder, string connectionString)
        {
            return builder.ConfigureScheduleProviderFactory(new SqlScheduleProviderFactory(connectionString));
        }
    }
}