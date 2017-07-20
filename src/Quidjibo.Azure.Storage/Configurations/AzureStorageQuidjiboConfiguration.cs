using System.Collections.Generic;
using Quidjibo.Configurations;

namespace Quidjibo.Azure.Storage.Configurations
{
    public class AzureStorageQuidjiboConfiguration : IQuidjiboConfiguration
    {
        public List<string> Queues { get; }
        public bool SingleLoop { get; }
        public int PollingInterval { get; }
        public int MaxAttempts { get; }
        public int LockInterval { get; }
        public int Throttle { get; }
    }
}