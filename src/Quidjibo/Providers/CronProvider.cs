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

            var years = new[] { start.Year, start.Year + 1 };
            var months = ParsePart(parts[3], "1-12");
            var days = ParsePart(NormalizeMonths(parts[2]), "1-31");
            var hours = ParsePart(parts[1], "0-23");
            var minutes = ParsePart(parts[0], "0-59");
            var daysOfTheWeek = ParsePart(NormalizeDayOfWeek(parts[4]), "0-6");

            foreach (var year in years)
            {
                foreach (var month in months)
                {
                    foreach (var day in days)
                    {
                        if (day > MaxDayOfMonth(year, month))
                        {
                            continue;
                        }
                        foreach (var hour in hours)
                        {
                            foreach (var minute in minutes)
                            {
                                var date = new DateTime(year, month, day, hour, minute, 0);
                                if (date >= start && daysOfTheWeek.Contains((int)date.DayOfWeek))
                                {
                                    yield return date;
                                }
                            }
                        }
                    }
                }
            }
        }

        private int[] ParsePart(string part, string wild)
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

            return qry.OrderBy(x => x).ToArray();
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