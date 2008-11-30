using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;
using LinFu.IoC.Configuration;

namespace SampleLibrary.IOC
{
    [Preprocessor]
    public class SamplePreprocessor : IPreProcessor
    {
        public void Preprocess(IServiceRequest result)
        {
            // Do nothing
        }
    }
}
