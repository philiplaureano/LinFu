using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents the parameters used to add interception to a given method body.
    /// </summary>
    public class MethodBodyRewriterParameters : IMethodBodyRewriterParameters
    {
        private readonly CilWorker _cilWorker;
        private readonly VariableDefinition _interceptionDisabled;
        private readonly VariableDefinition _invocationInfo;
        private readonly VariableDefinition _returnValue;
        private readonly VariableDefinition _aroundInvokeProvider;
        private readonly VariableDefinition _methodReplacementProvider;
        private readonly VariableDefinition _classMethodReplacementProvider;
        private readonly IEnumerable<Instruction> _oldInstructions;
        private readonly Func<ModuleDefinition, MethodReference> _getMethodReplacementProviderMethod;
        private readonly Type _registryType;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodBodyRewriterParameters"/> class.
        /// </summary>
        /// <param name="IL">The CilWorker that is responsible for the current method body.</param>
        /// <param name="oldInstructions">The value indicating the list of old instructions in the current method body.</param>
        /// <param name="interceptionDisabled">The value that determines whether or not interception is disabled.</param>
        /// <param name="invocationInfo">The local variable that will store the <see cref="IInvocationInfo"/> instance.</param>
        /// <param name="returnValue">The value indicating the local variable that will store the return value.</param>
        /// <param name="methodReplacementProvider">The <see cref="IMethodReplacementProvider"/> instance.</param>
        /// <param name="aroundInvokeProvider">The <see cref="IAroundInvokeProvider"/> instance.</param>
        /// <param name="classMethodReplacementProvider">The class-level<see cref="IMethodReplacementProvider"/> instance.</param>
        /// <param name="getMethodReplacementProviderMethod">The functor that resolves the GetMethodReplacementProvider method.</param>
        /// <param name="registryType">The interception registry type that will be responsible for handling class-level interception events.</param>
        public MethodBodyRewriterParameters(CilWorker IL, IEnumerable<Instruction> oldInstructions, 
            VariableDefinition interceptionDisabled, 
            VariableDefinition invocationInfo, 
            VariableDefinition returnValue,
            VariableDefinition methodReplacementProvider, 
            VariableDefinition aroundInvokeProvider, 
            VariableDefinition classMethodReplacementProvider, 
            Func<ModuleDefinition, MethodReference> getMethodReplacementProviderMethod, Type registryType)
        {
            _cilWorker = IL;
            _oldInstructions = oldInstructions;
            _interceptionDisabled = interceptionDisabled;
            _invocationInfo = invocationInfo;
            _returnValue = returnValue;
            _methodReplacementProvider = methodReplacementProvider;
            _aroundInvokeProvider = aroundInvokeProvider;
            _classMethodReplacementProvider = classMethodReplacementProvider;
            _getMethodReplacementProviderMethod = getMethodReplacementProviderMethod;
            _registryType = registryType;
        }

        /// <summary>
        /// Gets the value indicating the list of old instructions in the current method body.
        /// </summary>
        /// <value>The value indicating the list of old instructions in the current method body.</value>
        public IEnumerable<Instruction> OldInstructions
        {
            get { return _oldInstructions; }
        }

        /// <summary>
        /// Gets the value indicating the class-level<see cref="IMethodReplacementProvider"/> instance.
        /// </summary>
        /// <value>The class-level<see cref="IMethodReplacementProvider"/> instance.</value>
        public VariableDefinition ClassMethodReplacementProvider
        {
            get { return _classMethodReplacementProvider; }
        }

        /// <summary>
        /// Gets the value indicating the local variable used to store the <see cref="IAroundInvokeProvider"/> instance.
        /// </summary>
        /// <value>The <see cref="IAroundInvokeProvider"/> instance.</value>
        public VariableDefinition AroundInvokeProvider
        {
            get { return _aroundInvokeProvider; }
        }

        /// <summary>
        /// Gets the value indicating the local variable used to store the <see cref="IMethodReplacementProvider"/> instance.
        /// </summary>
        /// <value>The <see cref="IMethodReplacementProvider"/> instance.</value>
        public VariableDefinition MethodReplacementProvider
        {
            get { return _methodReplacementProvider; }
        }

        /// <summary>
        /// Gets the value indicating the TargetMethod to be modified.
        /// </summary>
        /// <value>The method to be modified.</value>
        public MethodDefinition TargetMethod
        {
            get { return _cilWorker.GetMethod(); }
        }

        /// <summary>
        /// Gets the value indicating the local variable that will store the value that determines whether or not
        /// interception is disabled.
        /// </summary>
        /// <value>The value that determines whether or not interception is disabled.</value>
        public VariableDefinition InterceptionDisabled
        {
            get { return _interceptionDisabled; }
        }

        /// <summary>
        /// Gets the value indicating the local variable that will store the <see cref="IInvocationInfo"/> instance.
        /// </summary>
        /// <value>The local variable that will store the <see cref="IInvocationInfo"/> instance.</value>
        public VariableDefinition InvocationInfo
        {
            get { return _invocationInfo; }
        }

        /// <summary>
        /// Gets the value indicating the local variable that will store the return value.
        /// </summary>
        /// <value>The value indicating the local variable that will store the return value.</value>
        public VariableDefinition ReturnValue
        {
            get { return _returnValue; }
        }

        /// <summary>
        /// Gets the value indicating the interception registry type that will be responsible for handling class-level interception events.
        /// </summary>
        /// <value>The interception registry type that will be responsible for handling class-level interception events.</value>
        public Type RegistryType
        {
            get { return _registryType; }
        }

        /// <summary>
        /// Gets the value indicating the functor that resolves the GetMethodReplacementProvider method.
        /// </summary>
        /// <value>The functor that resolves the GetMethodReplacementProvider method.</value>
        public Func<ModuleDefinition, MethodReference> GetMethodReplacementProviderMethod
        {
            get { return _getMethodReplacementProviderMethod; }
        }
    }
}
