using System.Collections.Generic;

namespace Quidjibo.Models
{
    public class WorkPayload
    {
        public object Content { get; set; }
        public string Type { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}