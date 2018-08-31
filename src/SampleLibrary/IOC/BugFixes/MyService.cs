using System;
using LinFu.IoC.Configuration;

namespace SampleLibrary.IOC.BugFixes
{
    [Implements(typeof(IMyService))]
    public class MyService : IMyService
    {
        public void Foo()
        {
            throw new NotImplementedException();
        }
    }
}