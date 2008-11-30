using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IMethodBuilder{TMethod}"/> interface.
    /// </summary>
    /// <typeparam name="TMethod">The method type to generate.</typeparam>
    public abstract class BaseMethodBuilder<TMethod> : IMethodBuilder<TMethod>
        where TMethod : MethodBase
    {
        /// <summary>
        /// Creates a method from the <paramref name="existingMethod"/>.
        /// </summary>
        /// <param name="existingMethod">The method that will be used to define the new method.</param>
        /// <returns>A method based on the old method.</returns>
        public MethodBase CreateMethod(TMethod existingMethod)
        {
            var returnType = GetReturnType(existingMethod);
            var parameterTypes = (from p in existingMethod.GetParameters()
                                  select p.ParameterType).ToArray();

            // Determine the method signature
            IList<Type> parameterList = GetParameterList(existingMethod, parameterTypes);

            var declaringType = existingMethod.DeclaringType;
            var module = declaringType.Module;
            var dynamicMethod = new DynamicMethod(string.Empty, returnType, parameterList.ToArray(), module);
            var IL = dynamicMethod.GetILGenerator();

            // Push the target instance, if necessary
            PushInstance(IL, existingMethod);

            // Push the method arguments
            PushMethodArguments(IL, existingMethod);

            // Call the target method
            EmitCall(IL, existingMethod);

            // Unbox the return type
            IL.Emit(OpCodes.Ret);

            return dynamicMethod;
        }

        /// <summary>
        /// Pushes the method arguments onto the stack.
        /// </summary>
        /// <param name="IL">The <see cref="ILGenerator"/> of the target method body.</param>
        /// <param name="targetMethod">The target method that will be invoked.</param>
        protected virtual void PushMethodArguments(ILGenerator IL, MethodBase targetMethod)
        {
            var parameterTypes = (from p in targetMethod.GetParameters()
                                  select p.ParameterType).ToArray();

            // Push the method arguments onto the stack
            var parameterCount = parameterTypes.Length;
            for (var index = 0; index < parameterCount; index++)
            {
                IL.Emit(OpCodes.Ldarg, index);
            }
        }

        /// <summary>
        /// Determines the parameter types of the dynamically generated method.
        /// </summary>
        /// <param name="existingMethod">The target method.</param>
        /// <param name="parameterTypes">The target method argument types.</param>
        /// <returns>The list of <see cref="System.Type"/> objects that describe the signature of the method to generate.</returns>
        protected virtual IList<Type> GetParameterList(TMethod existingMethod, Type[] parameterTypes)
        {            
            return new List<Type>(parameterTypes);
        }

        /// <summary>
        /// Pushes the method target onto the stack.
        /// </summary>
        /// <param name="IL">The <see cref="ILGenerator"/> that belongs to the method body.</param>
        /// <param name="method">The current method.</param>
        protected virtual void PushInstance(ILGenerator IL, TMethod method)
        {
        }

        /// <summary>
        /// Determines the return type from the target <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The target method itself.</param>
        /// <returns>The method return type.</returns>
        protected abstract Type GetReturnType(TMethod method);

        /// <summary>
        /// Emits the instruction to call the target <paramref name="method"/>
        /// </summary>
        /// <param name="IL">The <see cref="ILGenerator"/> of the target method body.</param>
        /// <param name="method">The method that will be invoked.</param>
        protected abstract void EmitCall(ILGenerator IL, TMethod method);
    }
}
