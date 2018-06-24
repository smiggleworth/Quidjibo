using Quidjibo.Models;

namespace Quidjibo.Extensions
{
    public static class ScheduleItemExtensions
    {
        public static bool EquivalentTo(this ScheduleItem item, ScheduleItem existingItem)
        {
            return existingItem != null && item.Name == existingItem.Name
                                        && item.CronExpression == existingItem.CronExpression
                                        && item.Queue == existingItem.Queue;
        }
    }
}