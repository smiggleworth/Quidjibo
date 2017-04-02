using System.Collections.Generic;

namespace Quidjibo.Models
{
    public class WorkflowPayload
    {
        public int Step { get; set; }
        public int CurrentStep { get; set; }
        public Dictionary<int, IEnumerable<WorkPayload>> Entries { get; set; }
    }
}