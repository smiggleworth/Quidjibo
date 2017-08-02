using System;
using Quidjibo.Misc;
using Quidjibo.Models;

namespace Quidjibo.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ScheduleAttribute : Attribute
    {
        /// <summary>
        ///     The unique name of the scheduled job
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The name of the queue
        /// </summary>
        public string Queue { get; }

        /// <summary>
        ///     The cron to parse for scheduling
        /// </summary>
        public Cron Cron { get; }

        /// <summary>
        ///     The client key to restrict which configuration this should be included
        /// </summary>
        public Type ClientKey { get; }


        public ScheduleAttribute(string name, string expression)
            : this(name, expression, "default") { }

        public ScheduleAttribute(string name, string expression, string queue)
            : this(name, expression, queue, typeof(DefaultClientKey)) { }

        public ScheduleAttribute(string name, string expression, string queue, Type clientKey)
            : this(name, new Cron(expression), queue, clientKey) { }

        protected ScheduleAttribute(string name, Cron cron, string queue, Type clientKey)
        {
            Name = name;
            Queue = queue;
            Cron = cron;
            ClientKey = clientKey;
        }
    }
}