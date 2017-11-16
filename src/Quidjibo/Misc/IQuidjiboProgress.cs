using System;
using Quidjibo.Models;

namespace Quidjibo.Misc
{
    public interface IQuidjiboProgress : IProgress<Tracker>
    {
        void Report(int value, string text);
    }
}