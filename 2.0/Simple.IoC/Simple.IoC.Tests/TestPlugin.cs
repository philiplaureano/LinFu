using System;
using System.Collections.Generic;
using System.Text;
using Simple.IoC.Loaders;

namespace Simple.IoC.Tests
{
    [ContainerPlugin]
    public class TestPlugin : IContainerPlugin
    {
        private static int beginLoadCount = 0;
        private static int endLoadCount = 0;
        #region IContainerPlugin Members

        public void BeginLoad(IContainer container)
        {
            beginLoadCount++;
        }

        public void EndLoad(IContainer container)
        {
            endLoadCount++;
        }

        #endregion
        
        public static void Reset()
        {
            beginLoadCount = 0;
            endLoadCount = 0;
        }
        public static bool IsCalled
        {
            get { return beginLoadCount > 0 && endLoadCount > 0; }
        }
    }
}
