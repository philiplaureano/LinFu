using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC.Loaders
{
    public enum LifecycleType
    {
        OncePerRequest = 0,
        OncePerThread = 1,
        Singleton = 2
    }
}
