using System;

namespace Quidjibo.Servers
{
    public interface IWorkServer : IDisposable
    {
        string Worker { get; }
        void Start();
        void Stop();
    }
}