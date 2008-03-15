using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.Delegates
{
    public interface IDeferredArgument
    {
        object Evaluate();
    }
}
