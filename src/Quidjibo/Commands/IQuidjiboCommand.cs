using System.Collections.Generic;

namespace Quidjibo.Commands
{
    public interface IQuidjiboCommand
    {
        Dictionary<string, string> Metadata { get; set; }
    }
}