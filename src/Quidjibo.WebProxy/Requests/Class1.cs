using System;
using System.Collections.Generic;
using System.Text;

namespace Quidjibo.WebProxy.Requests
{
    public class ReceiveWorkRequest
    {
        public string Worker { get; set; }
        public string[] Queues { get; set; }
    }
}
