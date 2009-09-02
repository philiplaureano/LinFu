using System;
using System.Linq;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil.Factories
{
    /// <summary>
    /// A factory instance that creates <see cref="Action{T}"/> delegates
    /// that emit the necessary <see cref="IInvocationInfo"/> information
    /// and store it in a local variable named '__invocationInfo___'.
    /// </summary>
    [Factory(typeof(Action<MethodDefinition>), ServiceName = "AddInvocationInfo")]
    public class AddInvocationInfoActionFactory : IFactory
    {
        /// <summary>
        /// Generates the <see cref="Action{T}"/> delegate that will emit
        /// the necessary <see cref="IInvocationInfo"/> information.
        /// </summary>
        /// <param name="request">The <see cref="IFactoryRequest"/> instance that describes the requested service type.</param>
        /// <returns>A delegate that can emit the necessary <see cref="IInvocationInfo"/> context that will allow other developers to infer information about the method currently being executed.</returns>
        public object CreateInstance(IFactoryRequest request)
        {
            var container = request.Container;
            Action<MethodDefinition> result =
                method =>
                {
                    var body = method.Body;

                    // Add the IInvocationInfo 
                    // instance only once
                    var localAlreadyExists = (from VariableDefinition local in body.Variables
                                  where local.Name == "___invocationInfo___"
                                  select local).Count() > 0;

                    if (localAlreadyExists)
                        return;

                    var variable = method.AddLocal<IInvocationInfo>();
                    variable.Name = "___invocationInfo___";

                    var emitInfo = (IEmitInvocationInfo)container.GetService(typeof(IEmitInvocationInfo));
                    emitInfo.Emit(method, method, variable);
                };

            return result;
        }
    }
}
