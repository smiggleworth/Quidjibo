using System;
using System.Collections.Generic;
using System.Linq;

namespace Quidjibo.Providers
{
    public class CronProvider : ICronProvider
    {
        /***** https://en.wikipedia.org/wiki/Cron *****
         
        ┌───────────── minute(0 - 59)
        │ ┌───────────── hour(0 - 23)
        │ │ ┌───────────── day of month(1 - 31)
        │ │ │ ┌───────────── month(1 - 12)
        │ │ │ │ ┌───────────── day of week(0 - 6) (Sunday to Saturday;)
        │ │ │ │ │                                       
        │ │ │ │ │
        │ │ │ │ │
        * * * * *  command to execute *****/

        public DateTime GetNextSchedule(string expression)
        {
            return GetNextSchedule(expression, DateTime.UtcNow);
        }

        public DateTime GetNextSchedule(string expression, DateTime start)
        {
            return GetSchedule(expression).First(x => x > start);
        }

        public IEnumerable<DateTime> GetSchedule(string expression)
        {
            return GetSchedule(expression, DateTime.UtcNow);
        }

        public IEnumerable<DateTime> GetSchedule(string expression, DateTime start)
        {
            var parts = expression.Split(new[]
            {
                ' '
            }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 5)
            {
                throw new InvalidOperationException("Expression must contain 5 parts");
            }

            return from y in new[]
                   {
                       start.Year,
                       start.Year + 1
                   }
                   from mo in ParsePart(parts[3], "1-12")
                   from dom in ParsePart(NormalizeMonths(parts[2]), "1-31")
                   from h in ParsePart(parts[1], "0-23")
                   from m in ParsePart(parts[0], "0-59")
                   where dom <= MaxDayOfMonth(y, mo)
                   let date = new DateTime(y, mo, dom, h, m, 0)
                   where date >= start && ParsePart(NormalizeDayOfWeek(parts[4]), "0-6").Contains((int)date.DayOfWeek)
                   orderby date
                   select date;
        }

        private static IEnumerable<int> ParsePart(string part, string wild)
        {
            if (part.Contains("*"))
            {
                part = part.Replace("*", wild);
            }

            var qry = from sp in part.Split(',')
                      let stepParts = sp.Split('/')
                      let step = stepParts.Length == 2 ? int.Parse(stepParts[1]) : 1
                      let range = stepParts[0].Split('-').Select(int.Parse).ToArray()
                      from value in GetSteps(range[0], range[range.Length - 1], step)
                      select value;

            return qry;
        }


        private static int MaxDayOfMonth(int year, int month)
        {
            if (month == 2)
            {
                return DateTime.IsLeapYear(year) ? 29 : 28;
            }

            return month == 4 || month == 6 || month == 9 || month == 11 ? 30 : 31;
        }

        private static IEnumerable<int> GetSteps(int min, int max, int step)
        {
            if (min == max)
            {
                yield return min;

                yield break;
            }

            var prev = 0;
            for (var i = min; i <= max; i++)
            {
                var diff = i - prev;
                if (diff % step == 0)
                {
                    yield return i;

                    prev = i;
                }
            }
        }

        private static string NormalizeMonths(string months)
        {
            return months.ToUpper()
                         .Replace("JAN", "1")
                         .Replace("FEB", "2")
                         .Replace("MAR", "3")
                         .Replace("APR", "4")
                         .Replace("MAY", "5")
                         .Replace("JUN", "6")
                         .Replace("JUL", "7")
                         .Replace("AUG", "8")
                         .Replace("SEP", "9")
                         .Replace("OCT", "10")
                         .Replace("NOV", "11")
                         .Replace("DEC", "12");
        }

        private static string NormalizeDayOfWeek(string dayOfWeek)
        {
            return dayOfWeek.ToUpper()
                            .Replace("SUM", "0")
                            .Replace("MON", "1")
                            .Replace("TUE", "2")
                            .Replace("WED", "3")
                            .Replace("THUR", "4")
                            .Replace("FRI", "5")
                            .Replace("SAT", "6");
        }
    }
}