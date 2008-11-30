using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarLibrary3;
using LinFu.IoC.Configuration;

namespace CarLibrary4
{
    [Implements(typeof(IEngine), LifecycleType.OncePerRequest, ServiceName = "OldEngine")]
    public class OldEngine : IEngine
    {
        #region IEngine Members

        public void Start()
        {
            Console.WriteLine("Old Engine Started!");
        }

        public void Stop()
        {
            Console.WriteLine("Old Engine Stopped!");
        }

        #endregion
    }
}
