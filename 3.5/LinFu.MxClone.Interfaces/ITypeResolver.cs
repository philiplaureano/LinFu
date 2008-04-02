using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.MxClone.Interfaces
{
    public interface ITypeResolver
    {
        Type Resolve(string typename, string assemblyQualifiedName);
    }
}
