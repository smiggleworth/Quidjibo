using System;
using System.Collections.Generic;
using Quidjibo.Commands;

namespace Quidjibo.SimpleInjector.Tests.Samples
{
    public class BasicCommand : IQuidjiboCommand
    {
        public Guid? CorrelationId { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}