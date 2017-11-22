using System;
using Quidjibo.Misc;
using Quidjibo.Models;

namespace Quidjibo.Attributes {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class WeeklyScheduleAttribute : ScheduleAttribute
    {
        public WeeklyScheduleAttribute(string name, DayOfWeek dayOfTheWeek, int hour, int minute)
            : this(name, dayOfTheWeek, hour, minute, "default") { }

        public WeeklyScheduleAttribute(string name, DayOfWeek dayOfTheWeek, int hour, int minute, string queue)
            : this(name, dayOfTheWeek, hour, minute, queue, typeof(DefaultClientKey)) { }

        public WeeklyScheduleAttribute(string name, DayOfWeek dayOfTheWeek, int hour, int minute, string queue, Type clientKey)
            : base(name, Cron.Weekly(dayOfTheWeek, hour, minute), queue, clientKey) { }
    }
}