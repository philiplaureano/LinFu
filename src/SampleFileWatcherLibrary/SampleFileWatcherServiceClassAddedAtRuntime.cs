using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration;
using SampleLibrary;

namespace SampleFileWatcherLibrary
{
    [Implements(typeof(ISampleService))]
    public class SampleFileWatcherServiceClassAddedAtRuntime : ISampleService 
    {
        public void DoSomething()
        {
            throw new NotImplementedException();
        }
    }
}
