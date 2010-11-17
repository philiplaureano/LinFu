using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
