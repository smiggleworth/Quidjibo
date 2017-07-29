using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Quidjibo.Owin.Middleware
{
    public class QuidjiboProxyServerMiddleware : OwinMiddleware
    {


        public QuidjiboProxyServerMiddleware(OwinMiddleware next, QuidjiboBuilder quidjiboBuilder) : base(next)
        {
            //_quidjiboBuilder = quidjiboBuilder;
        }

        public override Task Invoke(IOwinContext context)
        {
            throw new NotImplementedException();
        }
    }
}
