using System;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;
using Mono.Cecil;

namespace LinFu.AOP.Cecil.Factories
{
    /// <summary>
    ///     Represents a class that generates <see cref="Action{T1,T2}" /> instances
    ///     that apply a specific method weaver (with the name given in the first delegate parameter)
    ///     to every type in every module of an <see cref="AssemblyDefinition" /> instance.
    /// </summary>
    [Factory(typeof(Action<string, AssemblyDefinition>), ServiceName = "AssemblyWeaver")]
    public class AssemblyWeaverActionFactory : IFactory
    {
        /// <summary>
        ///     Generates <see cref="Action{T1, T2}" /> instances
        ///     that apply a specific method weaver (with the name given in the first delegate parameter)
        ///     to every type in every module of an <see cref="AssemblyDefinition" /> instance.
        /// </summary>
        /// <param name="request">The <see cref="IFactoryRequest" /> that describes the service request.</param>
        /// <returns>An action delegate that will apply a specific method weaver to all the types in the given assembly.</returns>
        public object CreateInstance(IFactoryRequest request)
        {
            var container = request.Container;
            Action<string, AssemblyDefinition> result =
                (weaverName, assembly) =>
                {
                    // Create the lambda that can modify the target types
                    var weaveWith =
                        (Action<string, TypeDefinition>)
                        container.GetService("TypeWeaver", typeof(Action<string, TypeDefinition>));
                    var mainModule = assembly.MainModule;

                    foreach (TypeDefinition type in mainModule.Types)
                        // Use the method weaver on the target type
                        weaveWith(weaverName, type);
                };

            return result;
        }
    }
}