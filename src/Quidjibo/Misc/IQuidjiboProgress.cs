using System;

namespace Quidjibo.Models
{
    public interface IQuidjiboProgress : IProgress<Tracker>
    {
        void Report(int value, string text);
    }
}