﻿using Quidjibo.Configurations;

namespace Quidjibo.WebProxy.Configurations
{
    public class WebProxyQuidjiboConfiguration : IQuidjiboConfiguration
    {
        /// <summary>
        ///     The Url to the Web Server Hosting a web proxy
        /// </summary>
        public string Url { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public int PollingInterval { get; set; }

        public WebProxyQuidjiboConfiguration()
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
        public bool EnableWorker { get; set; }
        public bool EnableScheduler { get; set; }
    }
}