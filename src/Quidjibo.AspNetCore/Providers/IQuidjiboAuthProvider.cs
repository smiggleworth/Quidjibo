using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quidjibo.AspNetCore.Providers
{
    public interface IQuidjiboAuthProvider
    {
        Task<bool> AuthenticateAsync(string key);
    }
}
