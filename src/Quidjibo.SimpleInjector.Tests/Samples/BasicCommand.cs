using System.Collections.Generic;
using Quidjibo.Commands;

namespace Quidjibo.SimpleInjector.Tests.Samples
{
    public class BasicCommand : IQuidjiboCommand
    {
        public Dictionary<string, string> Metadata { get; set; }
    }
}