using System;
using Quidjibo.Misc;
using Quidjibo.Models;

namespace Quidjibo.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScheduleAttribute : Attribute
    {
        public IQuidjiboClientKey ClientKey { get; set; }

        public Cron Cron { get; set; }

        public ScheduleAttribute(string expression, IQuidjiboClientKey clientKey)
        {
            Cron = new Cron(expression);
            ClientKey = clientKey;
        }
    }
}