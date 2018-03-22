using System;
using System.Collections.Generic;
using System.Text;

namespace Quidjibo.DataProtection.Exceptions
{
    public class MacMismatchException : Exception
    {
        public MacMismatchException()
        {
        }

        public MacMismatchException(string message)
            : base(message)
        {
        }

        public MacMismatchException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
