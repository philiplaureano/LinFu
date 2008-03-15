using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace LinFu.AOP.Weavers.Cecil
{
    public interface IMethodFilter
    {
        bool ShouldWeave(MethodDefinition method);
    }
}
