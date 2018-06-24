using System;
using System.Collections.Generic;
using Quidjibo.Attributes;
using Quidjibo.Commands;

namespace Quidjibo.Tests.Samples
{
    [QueueName("custom-queue-name")]
    public class BasicWithAttributeCommand : IQuidjiboCommand
    {
        public Guid? CorrelationId { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}