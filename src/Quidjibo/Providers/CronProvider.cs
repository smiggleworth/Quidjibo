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
            return GetSchedule(expression, start).First(x => x > start);
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

            var years = new[] {start.Year, start.Year + 1};
            var months = ParsePart(parts[3], "1-12").ToArray();
            var days = ParsePart(NormalizeMonths(parts[2]), "1-31").ToArray();
            var hours = ParsePart(parts[1], "0-23").ToArray();
            var minutes = ParsePart(parts[0], "0-59").ToArray();
            var daysOfTheWeek = ParsePart(NormalizeDayOfWeek(parts[4]), "0-6").ToArray();

            var qry = from y in years
                      from mo in months
                      from dom in days
                      where dom <= MaxDayOfMonth(y, mo)
                      from h in hours
                      from m in minutes
                      let date = new DateTime(y, mo, dom, h, m, 0)
                      where date >= start
                            && daysOfTheWeek.Contains((int)date.DayOfWeek)
                      orderby date
                      select date;
            return qry;
        }

        private IEnumerable<int> ParsePart(string part, string wild)
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


        private int MaxDayOfMonth(int year, int month)
        {
            if (month == 2)
            {
                return DateTime.IsLeapYear(year) ? 29 : 28;
            }

            return month == 4 || month == 6 || month == 9 || month == 11 ? 30 : 31;
        }

        private IEnumerable<int> GetSteps(int min, int max, int step)
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

        private string NormalizeMonths(string months)
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

        private string NormalizeDayOfWeek(string dayOfWeek)
        {
            return dayOfWeek.ToUpper()
                            .Replace("SUN", "0")
                            .Replace("MON", "1")
                            .Replace("TUE", "2")
                            .Replace("WED", "3")
                            .Replace("THUR", "4")
                            .Replace("FRI", "5")
                            .Replace("SAT", "6");
        }
    }
}