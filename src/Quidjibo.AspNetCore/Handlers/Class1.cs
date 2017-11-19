using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Quidjibo.AspNetCore.Handlers
{
    public class ReceiveWorkHandler
    {

        public Task Run(HttpContext context)
        {

            return Task.CompletedTask;
        }
    }
}
