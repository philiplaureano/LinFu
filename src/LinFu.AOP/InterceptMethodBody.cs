using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents a method rewriter type that adds interception capabilities to any given method body.
    /// </summary>
    public class InterceptMethodBody : BaseMethodRewriter
    {
        private readonly Func<MethodReference, bool> _methodFilter;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptMethodBody"/> class.
        /// </summary>
        /// <param name="methodFilter">The method filter that will determine the methods with the method bodies that will be intercepted.</param>
        public InterceptMethodBody(Func<MethodReference, bool> methodFilter)
        {
            _methodFilter = methodFilter;
        }

        /// <summary>
        /// Determines whether or not the given method should be modified.
        /// </summary>
        /// <param name="targetMethod">The target method.</param>
        /// <returns>A <see cref="bool"/> indicating whether or not a method should be rewritten.</returns>
        protected override bool ShouldRewrite(MethodDefinition targetMethod)
        {
            return _methodFilter(targetMethod);
        }

        /// <summary>
        /// Rewrites the instructions in the target method body.
        /// </summary>
        /// <param name="method">The target method.</param>
        /// <param name="IL">The <see cref="CilWorker"/> instance that represents the method body.</param>
        /// <param name="oldInstructions">The IL instructions of the original method body.</param>
        protected override void RewriteMethodBody(MethodDefinition method, CilWorker IL, IEnumerable<Instruction> oldInstructions)
        {
            var interceptionDisabled = method.AddLocal<bool>();
            var invocationInfo = method.AddLocal<IInvocationInfo>();
            var aroundInvokeProvider = method.AddLocal<IAroundInvokeProvider>();
            var methodReplacementProvider = method.AddLocal<IMethodReplacementProvider>();


            var returnValue = method.AddLocal<object>();
            var classMethodReplacementProvider = method.AddLocal<IMethodReplacementProvider>();

            var parameters = new MethodBodyRewriterParameters(IL, oldInstructions, interceptionDisabled, invocationInfo, returnValue, aroundInvokeProvider, methodReplacementProvider, classMethodReplacementProvider);

            var emitter = new InvocationInfoEmitter();
            IInstructionEmitter getInterceptionDisabled = new GetInterceptionDisabled(parameters);
            ISurroundMethodBody surroundMethodBody = new SurroundMethodBody(parameters);
            IInstructionEmitter getClassMethodReplacementProvider = new GetClassMethodReplacementProvider(parameters);
            IInstructionEmitter addMethodReplacement = new AddMethodReplacementImplementation(parameters);
            
            var rewriter = new InterceptAndSurroundMethodBody(emitter, getInterceptionDisabled, surroundMethodBody, getClassMethodReplacementProvider, addMethodReplacement, parameters);

            // Determine whether or not the method should be intercepted
            rewriter.Rewrite(method, IL, oldInstructions);
        }                                             
    }
}
