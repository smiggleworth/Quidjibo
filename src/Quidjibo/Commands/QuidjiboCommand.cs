using System.Collections.Generic;

namespace Quidjibo.Commands
{
    public abstract class QuidjiboCommand : IQuidjiboCommand
    {
        public Dictionary<string, string> Metadata { get; set; }
    }
}