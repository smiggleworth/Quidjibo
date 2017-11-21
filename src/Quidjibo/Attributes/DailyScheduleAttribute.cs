using System;
using Quidjibo.Misc;
using Quidjibo.Models;

namespace Quidjibo.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DailyScheduleAttribute : ScheduleAttribute
    {
        public DailyScheduleAttribute(string name, int hour, int minute)
            : this(name, hour, minute, "default") { }

        public DailyScheduleAttribute(string name, int hour, int minute, string queue)
            : this(name, hour, minute, queue, typeof(DefaultClientKey)) { }

        public DailyScheduleAttribute(string name, int hour, int minute, string queue, Type clientKey)
            : base(name, Cron.Daily(hour, minute), queue, clientKey) { }
    }
}