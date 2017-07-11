using System;
using Quidjibo.Misc;
using Quidjibo.Models;

namespace Quidjibo.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScheduleAttribute : Attribute
    {
        public Type ClientKey { get; set; }

        public Cron Cron { get; set; }

        public ScheduleAttribute(Cron cron)
            : this(cron, typeof(DefaultClientKey)) { }

        public ScheduleAttribute(string expression)
            : this(new Cron(expression), typeof(DefaultClientKey)) { }

        public ScheduleAttribute(string expression, Type type)
            : this(new Cron(expression), type) { }

        public ScheduleAttribute(Cron cron, Type type)
        {
            Cron = cron;
            ClientKey = type;
        }
    }
}