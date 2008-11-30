using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.IOC
{
    public interface ITargetHolder
    {
        object Target { get; set; }
    }
}
