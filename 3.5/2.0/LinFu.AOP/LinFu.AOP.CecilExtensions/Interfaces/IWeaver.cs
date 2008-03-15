using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace LinFu.AOP.CecilExtensions
{
    public interface IWeaver<T>
    {
        void ImportReferences(ModuleDefinition module);
        bool ShouldWeave(T item);
        void Weave(T item);
    }
}
