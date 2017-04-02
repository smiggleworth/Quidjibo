using System;

namespace Quidjibo.Models
{
    public class ProgressItem
    {
        public Guid Id { get; set; }
        public Guid WorkId { get; set; }
        public Guid CorrelationId { get; set; }
        public string Name { get; set; }
        public string Queue { get; set; }
        public string Note { get; set; }
        public int Value { get; set; }
        public DateTime RecordedOn { get; set; }
    }
}