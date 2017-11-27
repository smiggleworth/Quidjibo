using System.Collections.Generic;
using Quidjibo.Commands;
using Quidjibo.Models;

namespace Quidjibo.Tests.Samples
{
    public class BasicCommand : IQuidjiboCommand {
        public Dictionary<string, string> Metadata { get; set; }
    }
}