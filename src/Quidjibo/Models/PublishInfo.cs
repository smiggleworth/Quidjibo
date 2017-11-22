using System;
using System.Collections.Generic;
using System.Text;

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
