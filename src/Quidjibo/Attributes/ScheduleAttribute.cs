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
            : this(expression, "default")
        {
        }
        public ScheduleAttribute(string expression, string queue)
            : this(expression, queue, null)
        {
        }

        public ScheduleAttribute(string expression, string queue, string name)
            : this(expression, queue, name, typeof(DefaultClientKey))
        {
        }

        public ScheduleAttribute(string expression, string queue, string name, Type clientKey)
        {
            Name = name;
            Queue = queue;
            Cron = new Cron(expression);
            ClientKey = clientKey;
        }
    }
}