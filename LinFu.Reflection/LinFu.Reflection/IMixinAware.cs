using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.Reflection
{
    public interface IMixinAware
    {
        DynamicObject Self { get; set; }
    }
}
