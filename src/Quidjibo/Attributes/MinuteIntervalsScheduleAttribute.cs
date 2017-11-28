using System;
using Quidjibo.Misc;
using Quidjibo.Models;

namespace Quidjibo.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MinuteIntervalsScheduleAttribute : ScheduleAttribute
    {
        public MinuteIntervalsScheduleAttribute(string name, int minute)
            : this(name, minute, "default")
        {
        }

        public MinuteIntervalsScheduleAttribute(string name, int minute, string queue)
            : this(name, minute, queue, typeof(DefaultClientKey))
        {
        }

        public MinuteIntervalsScheduleAttribute(string name, int minute, string queue, Type clientKey)
            : base(name, Cron.MinuteIntervals(minute), queue, clientKey)
        {
        }
    }
}