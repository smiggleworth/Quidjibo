namespace Quidjibo.Models
{
    public class Cron
    {
        public string Expression { get; set; }

        public Cron(string expression)
        {
            Expression = expression;
        }

        public static Cron EveryXMinute(int minute = 0)
        {
            return new Cron($"*/{minute} * * * *");
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