using System;
using Quidjibo.Models;

namespace Quidjibo.Misc
{
    public class QuidjiboProgress : Progress<Tracker>, IQuidjiboProgress
    {
        public void Report(int value, string text)
        {
            OnReport(new Tracker(value, text));
        }
    }
}