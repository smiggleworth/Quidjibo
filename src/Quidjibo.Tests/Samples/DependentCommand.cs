using System;
using System.Collections.Generic;
using Quidjibo.Commands;

namespace Quidjibo.Tests.Samples
{
    public class DependentCommand : IQuidjiboCommand
    {
        public Guid? CorrelationId { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}