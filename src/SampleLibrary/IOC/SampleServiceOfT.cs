using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration;
using SampleLibrary.IOC;

namespace SampleLibrary.IOC
{
    [Implements(typeof(ISampleService<>), LifecycleType.OncePerRequest)]
    public class SampleService<T> : ISampleService<T>
    {
        public SampleService(string text, bool b)
        {
            Text = text;
            Bool = b;
        }

        public SampleService(int i, bool b)
        {
            Int = i;
            Bool = b;
        }

        public SampleService(int i, string text)
        {
            Int = i;
            Text = text;
        }

        public SampleService(int i, string text, bool b)
        {
            Int = i;
            Text = text;
            Bool = b;
        }

        public int Int { get; set; }
        public string Text { get; set; }
        public bool Bool { get; set; }
    }
}
