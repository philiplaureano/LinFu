using System;
using System.Reflection;
using System.Reflection.Emit;

namespace LinFu.DynamicProxy
{
    internal class DefaultArgumentHandler : IArgumentHandler
    {
        #region IArgumentHandler Members

        public void PushArguments(ParameterInfo[] parameters, ILGenerator IL, bool isStatic)
        {
            int parameterCount = parameters == null ? 0 : parameters.Length;

            // object[] args = new object[size];
            IL.Emit(OpCodes.Ldc_I4, parameterCount);
            IL.Emit(OpCodes.Newarr, typeof (object));
            IL.Emit(OpCodes.Stloc_S, 0);

            if (parameterCount == 0)
            {
                IL.Emit(OpCodes.Ldloc_S, 0);
                return;
            }

            // Populate the object array with the list of arguments
            int index = 0;
            int argumentPosition = 1;
            foreach (ParameterInfo param in parameters)
            {
                Type parameterType = param.ParameterType;
                // args[N] = argumentN (pseudocode)
                IL.Emit(OpCodes.Ldloc_S, 0);
                IL.Emit(OpCodes.Ldc_I4, index);

                // Zero out the [out] parameters
                if (param.IsOut)
                {
                    IL.Emit(OpCodes.Ldnull);
                    IL.Emit(OpCodes.Stelem_Ref);
                    argumentPosition++;
                    index++;
                    continue;
                }

                IL.Emit(OpCodes.Ldarg, argumentPosition);
                if (parameterType.IsValueType)
                    IL.Emit(OpCodes.Box, parameterType);

                IL.Emit(OpCodes.Stelem_Ref);

                index++;
                argumentPosition++;
            }
            IL.Emit(OpCodes.Ldloc_S, 0);
        }

        #endregion
    }
}