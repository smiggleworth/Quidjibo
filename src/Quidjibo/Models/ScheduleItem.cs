using System;

namespace Quidjibo.Models
{
    public class ScheduleItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Queue { get; set; }
        public string CronExpression { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? EnqueueOn { get; set; }
        public DateTime? EnqueuedOn { get; set; }
        public DateTime? VisibleOn { get; set; }
        public byte[] Payload { get; set; }
    }
}