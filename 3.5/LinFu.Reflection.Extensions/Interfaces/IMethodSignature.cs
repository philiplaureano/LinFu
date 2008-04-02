using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection.Extensions
{
    public interface IMethodSignature
    {
        IEnumerable<Type> ParameterTypes { get; }
        Type ReturnType { get; }
    }
}
