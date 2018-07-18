using System;
using System.Collections.Generic;

namespace Quidjibo.Commands
{
    public abstract class QuidjiboCommand : IQuidjiboCommand
    {
        public Guid? CorrelationId { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}