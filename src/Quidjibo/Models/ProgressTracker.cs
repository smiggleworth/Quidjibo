using System;

namespace Quidjibo.Models
{
    public class ProgressTracker : Progress<Tracker>
    {
        public WorkItem Item { get; }

        public ProgressTracker(WorkItem item)
        {
            Item = item;
        }
    }
}