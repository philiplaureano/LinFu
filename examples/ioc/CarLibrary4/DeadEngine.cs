using System;
using System.Collections.Generic;
using System.Text;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace CarLibrary3
{
    [Implements(typeof(IEngine), LifecycleType.OncePerRequest, ServiceName="DeadEngine")]
    public class DeadEngine : IEngine
    {
        #region IEngine Members

        public void Start()
        {
            Console.WriteLine("I can't move!");
        }

        public void Stop()
        {
            Console.WriteLine("The engine has already stopped! It's dead!");
        }

        #endregion
    }
}
