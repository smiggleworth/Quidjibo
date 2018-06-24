using System;

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