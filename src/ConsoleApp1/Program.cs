using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Models;
using Quidjibo.Providers;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            var provider = new CronProvider();

            while (true)
            {
                var next = provider.GetSchedule(Cron.MinuteIntervals().Expression).Count();

                Thread.Sleep(300);

            }
        }
    }
}
