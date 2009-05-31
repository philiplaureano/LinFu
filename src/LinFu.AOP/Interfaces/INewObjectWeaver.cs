using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil.Interfaces
{
    /// <summary>
    /// Represents a type that can emit IL that instantiates an object
    /// within a given method.
    /// </summary>
    public interface INewObjectWeaver : IHostWeaver<TypeDefinition>
    {
        /// <summary>
        /// Adds local variables to the <paramref name="hostMethod"/>.
        /// </summary>
        /// <param name="hostMethod">The target method.</param>
        void AddLocals(MethodDefinition hostMethod);

        /// <summary>
        /// Determines whether or not the object instantiation call to the <paramref name="constructor"/>
        /// should be instrumented.
        /// </summary>
        /// <param name="constructor">The constructor that will be used to instantiate the target type.</param>
        /// <param name="concreteType">The type to be created.</param>
        /// <param name="hostMethod">The method that contains the instantiation request.</param>
        /// <returns><c>true</c> if the call to the <c>new</c> operator should be intercepted; otherwise, it should return <c>false</c>.</returns>
        bool ShouldIntercept(MethodReference constructor, TypeReference concreteType, MethodReference hostMethod);

        /// <summary>
        /// Emits the necessary <paramref name="IL"/> necessary to instantiate
        /// the <paramref name="concreteType"/>.
        /// </summary>
        /// <param name="hostMethod">The method that contains the activation request.</param>
        /// <param name="IL">The CilWorker that will be used to replace the existing instructions in the method body.</param>
        /// <param name="targetConstructor">The constructor that is currently being used to instantiate the concrete type.</param>
        /// <param name="concreteType">The <see cref="System.Type"/> that describes the object type that needs to be instantiated.</param>
        void EmitNewObject(MethodDefinition hostMethod, CilWorker IL, MethodReference targetConstructor, TypeReference concreteType);
    }
}
