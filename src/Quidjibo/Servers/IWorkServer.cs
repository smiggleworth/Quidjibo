using System;

namespace Quidjibo.Servers
{
    public interface IWorkServer : IDisposable
    {
        void Start();
        void Stop();
    }
}