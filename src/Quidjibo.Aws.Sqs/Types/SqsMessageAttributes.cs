namespace Quidjibo.Aws.Sqs.Types
{
    public struct SqsMessageAttributes
    {
        public static readonly string CorrelationId = "correlation-id";
        public static readonly string Name = "name";
        public static readonly string Queue = "queue";
        public static readonly string ScheduleId = "schedule-id";
        public static readonly string WorkItemId = "work-item-id";
    }
}