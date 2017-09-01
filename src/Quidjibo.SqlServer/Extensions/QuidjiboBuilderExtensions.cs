using System.Collections.Generic;
using Quidjibo.SqlServer.Configurations;
using Quidjibo.SqlServer.Factories;

namespace Quidjibo.SqlServer.Extensions
{
    public static class QuidjiboBuilderExtensions
    {
        /// <summary>
        ///     Use Sql Server for Work, Progress, and Scheduled Jobs, and sets Quidjibo configuration
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static QuidjiboBuilder UseSqlServer(this QuidjiboBuilder builder, SqlServerQuidjiboConfiguration config)
        {
            return builder.Configure(config)
                          .ConfigureWorkProviderFactory(new SqlWorkProviderFactory(config.ConnectionString))
                          .ConfigureProgressProviderFactory(new SqlProgressProviderFactory(config.ConnectionString))
                          .ConfigureScheduleProviderFactory(new SqlScheduleProviderFactory(config.ConnectionString));
        }


        /// <summary>
        ///     Use Sql Server for Work, Progress, and Scheduled Jobs
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="connectionString"></param>
        /// <param name="queues"></param>
        /// <returns></returns>
        public static QuidjiboBuilder UseSqlServer(this QuidjiboBuilder builder, string connectionString, params string[] queues)
        {
            var queueList = new List<string>();
            if (queues == null || queues.Length == 0)
            {
                queueList.Add("default");
            }
            else
            {
                queueList.AddRange(queues);
            }
            var config = new SqlServerQuidjiboConfiguration
            {
                ConnectionString = connectionString,
                Queues = queueList
            };

            return builder.Configure(config)
                          .ConfigureWorkProviderFactory(new SqlWorkProviderFactory(connectionString))
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