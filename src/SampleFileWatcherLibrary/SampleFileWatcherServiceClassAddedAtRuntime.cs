using System;
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