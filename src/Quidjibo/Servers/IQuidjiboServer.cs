using System;

namespace Quidjibo.Servers
{
    public interface IQuidjiboServer : IDisposable
    {
        string Worker { get; }
        void Start();
        void Stop();
    }
}