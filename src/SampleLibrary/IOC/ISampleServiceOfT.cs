using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.IOC
{
    public interface ISampleService<T>
    {
        int Int { get; }
        string Text { get; }
        bool Bool { get; }
    }
}
