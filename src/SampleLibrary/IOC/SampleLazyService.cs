using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.IOC
{
    public class SampleLazyService : ISampleService
    {
        public static bool IsInitialized
        {
            get; private set;
        }

        public SampleLazyService ()
        {
            IsInitialized = true;
        }

        public void DoSomething()
        {
            IsInitialized = true;
        }

        public static void Reset()
        {
            IsInitialized = false;
        }
    }
}
