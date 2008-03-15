using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace LinFu.AOP.CecilExtensions
{
    public interface IMethodWeaver : IWeaver<MethodDefinition>
    {
        void AddAdditionalMembers(TypeDefinition typeDef);
    }
}
