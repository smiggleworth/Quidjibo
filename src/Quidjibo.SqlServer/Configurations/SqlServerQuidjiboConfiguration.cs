﻿using System.Collections.Generic;
using Quidjibo.Configurations;

namespace Quidjibo.SqlServer.Configurations
{
    public class SqlServerQuidjiboConfiguration : IQuidjiboConfiguration
    {
        /// <summary>
        ///     The ConnectionString to the Sql Server
        /// </summary>
        public string ConnectionString { get; set; }

        public List<string> PublisherQueues { get; set; }
        public int PollingInterval { get; set; }

        public SqlServerQuidjiboConfiguration()
        {
            Throttle = 10;
            LockInterval = 30;
            MaxAttempts = 5;
            SingleLoop = true;
        }

        public string[] Queues { get; set; }
        public bool SingleLoop { get; set; }
        public int MaxAttempts { get; set; }
        public int LockInterval { get; set; }
        public int Throttle { get; set; }
        public bool EnableWorker { get; set; } = true;
        public bool EnableScheduler { get; set; } = true;
    }
}