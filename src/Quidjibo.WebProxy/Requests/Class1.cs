using System;
using System.Collections.Generic;
using System.Text;

namespace Quidjibo.WebProxy.Requests
{
    public class RequestWrapper
    {
        public string Worker { get; set; }
        public string[] Queues { get; set; }

        public string GetQueues()
        {
            return string.Join(",", Queues);
        }
    }

    public class RequestWrapper<T> : RequestWrapper
    {
        public T Data { get; set; }
    }
}
