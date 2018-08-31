using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    ///     Represents a class that emits the call to obtain the <see cref="IAroundInvokeProvider" /> instance.
    /// </summary>
    public class GetAroundInvokeProvider : IInstructionEmitter
    {
        private readonly VariableDefinition _aroundInvokeProvider;
        private readonly string _providerName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GetAroundInvokeProvider" /> class.
        /// </summary>
        /// <param name="aroundInvokeProvider">The local variable that holds the <see cref="IAroundInvokeProvider" /> instance.</param>
        /// <param name="providerName">The name of the <see cref="IAroundInvokeProvider" /> property.</param>
        public GetAroundInvokeProvider(VariableDefinition aroundInvokeProvider, string providerName)
        {
            _aroundInvokeProvider = aroundInvokeProvider;
            _providerName = providerName;
        }


        /// <summary>
        ///     Emits the call to obtain the <see cref="IAroundInvokeProvider" /> instance.
        /// </summary>
        /// <param name="IL">The <see cref="CilWorker" /> pointing to the target method body.</param>
        public void Emit(CilWorker IL)
        {
            var method = IL.GetMethod();
            var module = IL.GetModule();

            // var aroundInvokeProvider = this.AroundInvokeProvider;
            var propertyName = string.Format("get_{0}", _providerName);
            var getAroundInvokeProvider = module.ImportMethod<IAroundInvokeHost>(propertyName);

            if (!method.HasThis)
            {
                IL.Emit(OpCodes.Ldnull);
                IL.Emit(OpCodes.Stloc, _aroundInvokeProvider);
                return;
            }

            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Callvirt, getAroundInvokeProvider);
            IL.Emit(OpCodes.Stloc, _aroundInvokeProvider);
        }
    }
}