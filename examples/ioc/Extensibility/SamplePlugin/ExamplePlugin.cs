using System;
using System.Collections.Generic;
using System.Text;
using LinFu.IoC.Interfaces;
using LinFu.Reflection;


namespace SamplePlugin
{
    [LoaderPlugin]
    public class ExamplePlugin : ILoaderPlugin<IServiceContainer>
    {
        #region ILoaderPlugin Members

        public void BeginLoad(IServiceContainer container)
        {
            // Do something useful here
            Console.WriteLine("Load started");
        }
        public void EndLoad(IServiceContainer container)
        {
            // Do something useful here too
            Console.WriteLine("Load completed");
        }

        #endregion
    }
}
