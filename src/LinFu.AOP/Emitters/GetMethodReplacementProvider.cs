using System;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    ///     Represents a class that emits the instructions that obtain an instance-level
    ///     <see cref="IMethodReplacementProvider" /> instance.
    /// </summary>
    public class GetMethodReplacementProvider : IInstructionEmitter
    {
        private readonly MethodDefinition _hostMethod;
        private readonly VariableDefinition _methodReplacementProvider;
        private readonly Func<ModuleDefinition, MethodReference> _resolveGetProviderMethod;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GetMethodReplacementProvider" /> class.
        /// </summary>
        /// <param name="methodReplacementProvider">
        ///     The local variable that contains the <see cref="IMethodReplacementProvider" />
        ///     instance.
        /// </param>
        /// <param name="hostMethod">The target method.</param>
        /// <param name="resolveGetProviderMethod">The functor that will resolve the GetProvider method.</param>
        public GetMethodReplacementProvider(VariableDefinition methodReplacementProvider, MethodDefinition hostMethod,
            Func<ModuleDefinition, MethodReference> resolveGetProviderMethod)
        {
            if (methodReplacementProvider.VariableType.FullName != typeof(IMethodReplacementProvider).FullName)
                throw new ArgumentException();

            _methodReplacementProvider = methodReplacementProvider;
            _hostMethod = hostMethod;
            _resolveGetProviderMethod = resolveGetProviderMethod;
        }


        /// <summary>
        ///     Emits the instructions that obtain the <see cref="IMethodReplacementProvider" /> instance.
        /// </summary>
        /// <param name="IL">The <see cref="CilWorker" /> instance.</param>
        public void Emit(CilWorker IL)
        {
            var method = _hostMethod;
            var declaringType = method.DeclaringType;
            var module = declaringType.Module;

            if (!method.HasThis)
            {
                IL.Emit(OpCodes.Ldnull);
                IL.Emit(OpCodes.Stloc, _methodReplacementProvider);
                return;
            }

            var getProvider = _resolveGetProviderMethod(module);
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Callvirt, getProvider);
            IL.Emit(OpCodes.Stloc, _methodReplacementProvider);
        }
    }
}