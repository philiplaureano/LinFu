using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// A class that dynamically generates calls to a <see cref="MethodInfo"/> instance.
    /// </summary>
    public class MethodBuilder : BaseMethodBuilder<MethodInfo>
    {
        /// <summary>
        /// Pushes the method target onto the evaluation stack.
        /// </summary>
        /// <param name="IL">The <see cref="ILGenerator"/> of the method body.</param>
        /// <param name="method">The target method.</param>
        protected override void PushInstance(ILGenerator IL, MethodInfo method)
        {
            if (method.IsStatic)
                return;

            IL.Emit(OpCodes.Ldarg_0);
        }

        protected override void PushMethodArguments(ILGenerator IL, MethodBase targetMethod)
        {
            var parameterTypes = (from p in targetMethod.GetParameters()
                                  select p.ParameterType).ToArray();

            int offset = targetMethod.IsStatic ? 0 : 1;
            // Push the method arguments onto the stack
            var parameterCount = parameterTypes.Length;
            for (var index = 0; index < parameterCount; index++)
            {
                IL.Emit(OpCodes.Ldarg, index + offset);
            }
        }

        /// <summary>
        /// Determines the return type from the target <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The target method itself.</param>
        /// <returns>The method return type.</returns>
        protected override Type GetReturnType(MethodInfo method)
        {
            return method.ReturnType;
        }

        /// <summary>
        /// Determines the parameter types of the dynamically generated method.
        /// </summary>
        /// <param name="existingMethod">The target method.</param>
        /// <param name="parameterTypes">The target method argument types.</param>
        /// <returns>The list of <see cref="System.Type"/> objects that describe the signature of the method to generate.</returns>
        /// <remarks>This override will add an additional parameter type to accomodate the method target.</remarks>
        protected override IList<Type> GetParameterList(MethodInfo existingMethod, Type[] parameterTypes)
        {
            var parameterList = new List<Type>();

            if (!existingMethod.IsStatic)
                parameterList.Add(existingMethod.DeclaringType);

            parameterList.AddRange(parameterTypes);

            return parameterList;
        }
        /// <summary>
        /// Emits the instruction to call the target <paramref name="method"/>
        /// </summary>
        /// <param name="IL">The <see cref="ILGenerator"/> of the target method body.</param>
        /// <param name="method">The method that will be invoked.</param>
        protected override void EmitCall(ILGenerator IL, MethodInfo method)
        {
            var callInstruction = method.IsVirtual ? OpCodes.Callvirt : OpCodes.Call;
            IL.Emit(callInstruction, method);
        }
    }
}
