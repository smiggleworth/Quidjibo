using System.Collections.Generic;
using Quidjibo.Commands;

namespace Quidjibo.Tests.Samples
{
    public class UnhandledCommand : IQuidjiboCommand
    {
        public Dictionary<string, string> Metadata { get; set; }
    }
}