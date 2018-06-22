using System;

namespace Quidjibo.Configurations
{
    public interface IQuidjiboConfiguration
    {
        /// <summary>
        ///     The queues
        /// </summary>
        string[] Queues { get; }

        /// <summary>
        ///     The number of listener
        /// </summary>
        bool SingleLoop { get; }

        /// <summary>
        ///     The max attempts to retry work
        /// </summary>
        int MaxAttempts { get; }

        /// <summary>
        ///     The number of items to receive at once.
        /// </summary>
        int BatchSize { get; }

        /// <summary>
        ///     The lock interval in seconds
        /// </summary>
        int LockInterval { get; }

        /// <summary>
        ///     The maximum number of concurrent calls to receive work
        /// </summary>
        int Throttle { get; }

        /// <summary>
        ///     Enable the worker
        /// </summary>
        bool EnableWorker { get; set; }

        /// <summary>
        /// The frequency to poll the work queue in seconds.
        /// </summary>
        int? WorkPollingInterval { get; set; }

        /// <summary>
        ///     Enable the scheduler
        /// </summary>
        bool EnableScheduler { get; set; }

        /// <summary>
        /// The frequency to poll the persisisted schedules in seconds. 
        /// </summary>
        int? SchedulePollingInterval { get; set; }
    }
}