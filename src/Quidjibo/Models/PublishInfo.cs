using System;

namespace Quidjibo.Models
{
    public class PublishInfo
    {
        public Guid Id { get; }
        public Guid CorrelationId { get; }

        public PublishInfo(Guid id, Guid correlationId)
        {
            Id = id;
            CorrelationId = correlationId;
        }
    }
}