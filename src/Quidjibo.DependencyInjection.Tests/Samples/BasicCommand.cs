using System.Collections.Generic;
using Quidjibo.Commands;

namespace Quidjibo.DependencyInjection.Tests.Samples
{
    public class BasicCommand : IQuidjiboCommand
    {
        public Dictionary<string, string> Metadata { get; set; }
    }
}