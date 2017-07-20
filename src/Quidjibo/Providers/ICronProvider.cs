using System;
using System.Collections.Generic;

namespace Quidjibo.Providers
{
    public interface ICronProvider
    {
        DateTime GetNextSchedule(string expression);
        DateTime GetNextSchedule(string expression, DateTime start);
        IEnumerable<DateTime> GetSchedule(string expression);
        IEnumerable<DateTime> GetSchedule(string expression, DateTime start);
    }
}