using System.Collections.Generic;

namespace Quidjibo.Configurations
{
    public interface IQuidjiboConfiguration
    {
        /// <summary>
        ///     The queues
        /// </summary>
        List<string> Queues { get; }

        /// <summary>
        ///     The number of listener
        /// </summary>
        bool SingleLoop { get; }

        /// <summary>
        ///     The max attempts to retry work
        /// </summary>
        int MaxAttempts { get; }

        /// <summary>
        ///     The lock interval in seconds
        /// </summary>
        int LockInterval { get; }

        /// <summary>
        ///     The maximum number of concurrent calls to receive work
        /// </summary>
        int Throttle { get; }
    }
}