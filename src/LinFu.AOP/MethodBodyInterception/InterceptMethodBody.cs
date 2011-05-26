using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="IL">The <see cref="ILProcessor"/> instance that represents the method body.</param>
        /// <param name="oldInstructions">The IL instructions of the original method body.</param>
        protected override void RewriteMethodBody(MethodDefinition method, ILProcessor IL,
                                                  IEnumerable<Instruction> oldInstructions)
        {
            if (IsExcluded(method))
            {
                AddOriginalInstructions(IL, oldInstructions);
                return;
            }

            VariableDefinition interceptionDisabled = method.AddLocal<bool>();
            VariableDefinition invocationInfo = method.AddLocal<IInvocationInfo>();
            VariableDefinition aroundInvokeProvider = method.AddLocal<IAroundInvokeProvider>();
            VariableDefinition methodReplacementProvider = method.AddLocal<IMethodReplacementProvider>();


            VariableDefinition returnValue = method.AddLocal<object>();
            VariableDefinition classMethodReplacementProvider = method.AddLocal<IMethodReplacementProvider>();

            Func<ModuleDefinition, MethodReference> getInstanceMethodReplacementProviderMethod =
                module => module.Import(typeof (IMethodReplacementHost).GetMethod("get_MethodBodyReplacementProvider"));

            var parameters = new MethodBodyRewriterParameters(IL,
                                                              oldInstructions,
                                                              interceptionDisabled,
                                                              invocationInfo, returnValue,
                                                              methodReplacementProvider,
                                                              aroundInvokeProvider,
                                                              classMethodReplacementProvider,
                                                              getInstanceMethodReplacementProviderMethod,
                                                              typeof (AroundMethodBodyRegistry));

            var emitter = new InvocationInfoEmitter(true);

            IInstructionEmitter getMethodReplacementProvider =
                new GetMethodReplacementProvider(methodReplacementProvider, method,
                                                 getInstanceMethodReplacementProviderMethod);

            IInstructionEmitter getInterceptionDisabled = new GetInterceptionDisabled(parameters);
            ISurroundMethodBody surroundMethodBody = new SurroundMethodBody(parameters, "AroundMethodBodyProvider");
            IInstructionEmitter getClassMethodReplacementProvider = new GetClassMethodReplacementProvider(parameters,
                                                                                                          module =>
                                                                                                          module.Import(
                                                                                                              typeof (
                                                                                                                  MethodBodyReplacementProviderRegistry
                                                                                                                  ).
                                                                                                                  GetMethod
                                                                                                                  ("GetProvider")));
            IInstructionEmitter addMethodReplacement = new AddMethodReplacementImplementation(parameters);

            var rewriter = new InterceptAndSurroundMethodBody(emitter, getInterceptionDisabled, surroundMethodBody,
                                                              getMethodReplacementProvider,
                                                              getClassMethodReplacementProvider, addMethodReplacement,
                                                              parameters);

            // Determine whether or not the method should be intercepted
            rewriter.Rewrite(method, IL, oldInstructions);
        }

        private void AddOriginalInstructions(ILProcessor IL, IEnumerable<Instruction> oldInstructions)
        {
            foreach (Instruction instruction in oldInstructions)
            {
                IL.Append(instruction);
            }
        }

        private bool IsExcluded(MethodDefinition method)
        {
            var excludedTypes = new[]
                                    {
                                        typeof (IMethodReplacementHost),
                                        typeof (IModifiableType), typeof (IActivatorHost),
                                        typeof (IFieldInterceptionHost), typeof (IAroundInvokeHost)
                                    };
            List<string> excludedMethods = (from type in excludedTypes
                                            from currentMethod in type.GetMethods()
                                            select currentMethod.Name).ToList();

            string methodName = method.Name;
            return excludedMethods.Contains(methodName);
        }
    }
}