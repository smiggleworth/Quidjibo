using System;

namespace Quidjibo.Models
{
    public class WorkItem
    {
        public Guid Id { get; set; }
        public Guid? ScheduleId { get; set; }
        public Guid CorrelationId { get; set; }
        public string Name { get; set; }
        public string Worker { get; set; }
        public string Queue { get; set; }
        public string Token { get; set; }
        public DateTime ExpireOn { get; set; }
        public DateTime? VisibleOn { get; set; }
        public int Attempts { get; set; }
        public byte[] Payload { get; set; }
        public object RawMessage { get; set; }
    }
}