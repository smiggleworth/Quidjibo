using System;

namespace Quidjibo.Exceptions
{
    public class QuidjiboNotInitializedException : Exception
    {
        public QuidjiboNotInitializedException() : base("The QuidjiboClient has not been initialized. This could be a timing issue or the BuildClient method was not invoked.") { }
    }
}