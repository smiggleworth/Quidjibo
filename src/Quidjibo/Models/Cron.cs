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

        /// <summary>
        /// Cron to run every x number of minutes
        /// </summary>
        /// <param name="minute"></param>
        /// <returns></returns>
        public static Cron MinuteIntervals(int minute = 1)
        {
            return new Cron($"*/{Math.Max(1, minute)} * * * *");
        }

        /// <summary>
        /// Cron to run daily at a given hour and minute
        /// </summary>
        /// <param name="hour">The hour of the day</param>
        /// <param name="minute">The minutes past the hour</param>
        /// <returns></returns>
        public static Cron Daily(int hour = 0, int minute = 0)
        {
            return new Cron($"{minute} {hour} * * *");
        }

        /// <summary>
        /// Cron to run monthly on given day of the week at a given hour and minute
        /// </summary>
        /// <param name="dayOfTheWeek"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <returns></returns>
        public static Cron Weekly(DayOfWeek dayOfTheWeek = DayOfWeek.Sunday, int hour = 0, int minute = 0)
        {
            return new Cron($"{minute} {hour} * * {(int)dayOfTheWeek}");
        }
    }
}