using System;
using LinFu.IoC.Configuration;
using SampleLibrary;

namespace SampleFileWatcherLibrary
{
    [Implements(typeof (ISampleService))]
    public class SampleFileWatcherServiceClassAddedAtRuntime : ISampleService
    {
        #region ISampleService Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}