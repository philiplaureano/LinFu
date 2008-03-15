using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.DynamicProxy
{
    public interface IWithTarget
    {
        object Target { get; set; }
    }
}
