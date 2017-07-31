using System;
using Quidjibo.Misc;
using Quidjibo.Models;

namespace Quidjibo.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScheduleAttribute : Attribute
    {
        /// <summary>
        /// The unique name of the scheduled job
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The name of the queue
        /// </summary>
        public string Queue { get; set; }

        /// <summary>
        /// The cron to parse for scheduling
        /// </summary>
        public Cron Cron { get; set; }

        /// <summary>
        /// The client key to restrict which configuration this should be included
        /// </summary>
        public Type ClientKey { get; set; }

        public ScheduleAttribute(string expression)
            : this(new Cron(expression)) { }

        public ScheduleAttribute(string expression, Type clientKey)
            : this(new Cron(expression), clientKey) { }

        public ScheduleAttribute(Cron cron)
            : this(cron, typeof(DefaultClientKey)) { }

        public ScheduleAttribute(Cron cron, Type clientKey)
            : this(null, null, cron, clientKey) { }

        public ScheduleAttribute(string name, string queue, Cron cron, Type clientKey)
        {
            Name = name;
            Queue = queue;
            Cron = cron;
            ClientKey = clientKey;
        }
    }
}