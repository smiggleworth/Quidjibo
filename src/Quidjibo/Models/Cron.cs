using System;

namespace Quidjibo.Models
{
    public class Cron
    {
        public string Expression { get; set; }

        public Cron(string expression)
        {
            Expression = expression;
        }

        public static Cron MinuteIntervals(int minute = 1)
        {
            return new Cron($"*/{Math.Max(1, minute)} * * * *");
        }

        public static Cron Daily(int hour = 0, int minute = 0)
        {
            return new Cron($"{minute} {hour} * * *");
        }

        public static Cron Weekly(int dayOfTheWeek = 0, int hour = 0, int minute = 0)
        {
            return new Cron($"{minute} {hour} * * {dayOfTheWeek}");
        }
    }
}