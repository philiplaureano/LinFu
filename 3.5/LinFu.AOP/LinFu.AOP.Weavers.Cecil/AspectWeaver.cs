using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.CecilExtensions;
using Mono.Cecil.Cil;
using Mono.Cecil;
using LinFu.AOP.Interfaces;
using System.Reflection;

namespace LinFu.AOP.Weavers.Cecil
{
    public partial class AspectWeaver : MethodPrologEpilogWeaver
    {
        private Instruction _skipOriginalCall;
        private InvocationContextBuilder _contextBuilder = new InvocationContextBuilder();
        public IMethodFilter MethodFilter { get; set; }
        public override bool ShouldWeave(MethodDefinition methodDef)
        {
            // By default, only public methods will be intercepted
            if (!methodDef.IsPublic && MethodFilter == null)
                return false;

            TypeReference declaringType = methodDef.DeclaringType;

            if (declaringType == _modifiableType)
                return false;

            List<string> ignoreList = new List<string> 
                { 
                    "AroundInvokeProvider",
                    "MethodReplacementProvider",
                    "IsInterceptionEnabled"
                };

            var matches = from propertyName in ignoreList
                          where methodDef.Name == string.Format("get_{0}", propertyName) ||
                          methodDef.Name == string.Format("set_{0}", propertyName)
                          select propertyName;

            if (matches.Count() > 0)
                return false;

            var results = from ParameterDefinition p in methodDef.Parameters
                          where p.IsOut || p.ParameterType.Name.EndsWith("&")
                          select p;

            // Methods with out parameters are not supported
            int count = results.ToList().Count;
            if (count > 0)
                return false;

            // Apply the method filter
            if (MethodFilter != null && !MethodFilter.ShouldWeave(methodDef))
                return false;

            return true;
        }
        public override void AddAdditionalMembers(TypeDefinition typeDef)
        {

            if (typeDef.Interfaces.Contains(_modifiableType))
                return;

            // Implement IModifiableType
            typeDef.Interfaces.Add(_modifiableType);
            typeDef.AddProperty("AroundInvokeProvider", typeof(IAroundInvokeProvider));
            typeDef.AddProperty("MethodReplacementProvider", typeof(IMethodReplacementProvider));
            typeDef.AddProperty("IsInterceptionEnabled", typeof(bool));
        }
        public override void AddLocals(MethodDefinition method)
        {
            _context = method.AddLocal(typeof(IInvocationContext));
            
            _aroundInvoke = method.AddLocal(typeof(IAroundInvoke));
            _methodReplacement = method.AddLocal(typeof(IMethodReplacement));
            _returnValue = method.AddLocal(typeof(object));
            _methodReplacementProvider = method.AddLocal(typeof(IMethodReplacementProvider));
            _aroundInvokeArray = method.AddLocal(typeof(IAroundInvoke[]));
            _aroundInvokeProvider = method.AddLocal(typeof(IAroundInvokeProvider));
        }
        public override IEnumerable<Instruction> CreateProlog(MethodDefinition methodDef, IEnumerable<Instruction> originalInstructions)
        {
            Queue<Instruction> instructions = new Queue<Instruction>();
            ModuleDefinition module = methodDef.DeclaringType.Module;

            CilWorker IL = methodDef.Body.CilWorker;
            Instruction callOriginalImplementation = IL.Create(OpCodes.Nop);

            if (!methodDef.IsStatic)
            {
                Instruction skipGetEnabledFlag = IL.Create(OpCodes.Nop);
                instructions.Enqueue(IL.Create(OpCodes.Ldarg_0));
                instructions.Enqueue(IL.Create(OpCodes.Isinst, _modifiableType));
                instructions.Enqueue(IL.Create(OpCodes.Brfalse, skipGetEnabledFlag));

                // if (!this.IsInterceptionEnabled)
                instructions.Enqueue(IL.Create(OpCodes.Ldarg_0));
                instructions.Enqueue(IL.Create(OpCodes.Isinst, _modifiableType));
                instructions.Enqueue(IL.Create(OpCodes.Callvirt, _isEnabled));

                //     return CallOriginalImplementation();
                instructions.Enqueue(IL.Create(OpCodes.Brfalse, callOriginalImplementation));
                instructions.Enqueue(skipGetEnabledFlag);
            }

            // Save the InstructionContext local variable                       
            _contextBuilder.BuildContext(IL, methodDef, _context, instructions);

            GetMethodReplacement(methodDef, instructions, IL);
            GetAroundInvoke(methodDef, instructions, IL);

            // if (aroundInvoke != null) {
            Instruction skipBeforeInvoke = IL.Create(OpCodes.Nop);
            instructions.Enqueue(IL.Create(OpCodes.Ldloc, _aroundInvoke));
            instructions.Enqueue(IL.Create(OpCodes.Brfalse, skipBeforeInvoke));

            // aroundInvoke.BeforeInvoke(context);
            instructions.Enqueue(IL.Create(OpCodes.Ldloc, _aroundInvoke));
            instructions.Enqueue(IL.Create(OpCodes.Ldloc, _context));
            instructions.Enqueue(IL.Create(OpCodes.Callvirt, _beforeInvoke));
            // }

            // Mark the SkipBeforeInvoke instruction label
            instructions.Enqueue(skipBeforeInvoke);

            // if (methodReplacement == null) {
            instructions.Enqueue(IL.Create(OpCodes.Ldloc, _methodReplacement));
            instructions.Enqueue(IL.Create(OpCodes.Brfalse, callOriginalImplementation));
            //     CallOriginalImplementation();
            // }

            // else 
            // {
            //     returnValue = methodReplacement.Invoke(context);
            TypeReference returnType = methodDef.ReturnType.ReturnType;

            instructions.Enqueue(IL.Create(OpCodes.Ldloc, _methodReplacement));
            instructions.Enqueue(IL.Create(OpCodes.Ldloc, _context));
            instructions.Enqueue(IL.Create(OpCodes.Callvirt, _invokeReplacement));
            IL.PackageReturnValue(module, instructions, returnType);
            // }

            _skipOriginalCall = IL.Create(OpCodes.Nop);
            instructions.Enqueue(IL.Create(OpCodes.Br, _skipOriginalCall));
            instructions.Enqueue(callOriginalImplementation);

            return instructions;
        }

        public override IEnumerable<Instruction> CreateEpilog(MethodDefinition methodDef, IEnumerable<Instruction> originalInstructions)
        {
            Queue<Instruction> instructions = new Queue<Instruction>();
            CilWorker IL = methodDef.Body.CilWorker;

            ModuleDefinition module = methodDef.DeclaringType.Module;
            Instruction skipEarlyReturn = IL.Create(OpCodes.Nop);

            Instruction lastInstruction = originalInstructions.LastOrDefault();
            TypeReference returnType = methodDef.ReturnType.ReturnType;

            Instruction skipToTheEnd = IL.Create(OpCodes.Nop);

            
            FlowControl flowControl = default(FlowControl);

            // Append a Ret instruction to empty method bodies
            // by default
            if (lastInstruction != null)
                flowControl = lastInstruction.OpCode.FlowControl;

            // If the last instruction is a Throw instruction,
            // then there's no need to insert a Ret instruction
            // to end the original method body
            if (lastInstruction == null || flowControl != FlowControl.Throw)
            {
                // if (aroundInvoke == null)
                instructions.Enqueue(IL.Create(OpCodes.Ldloc, _aroundInvoke));

                //    return CallOriginalImplementation();
                instructions.Enqueue(IL.Create(OpCodes.Brtrue, skipEarlyReturn));
                instructions.Enqueue(IL.Create(OpCodes.Ret));
            }

            instructions.Enqueue(_skipOriginalCall);
            instructions.Enqueue(skipEarlyReturn);


            if (returnType != _voidType && returnType.IsValueType)
                instructions.Enqueue(IL.Create(OpCodes.Box, returnType));

            if (returnType is GenericParameter)
                instructions.Enqueue(IL.Create(OpCodes.Box, returnType));

            if (returnType != _voidType)
                instructions.Enqueue(IL.Create(OpCodes.Stloc, _returnValue));


            Instruction skipAfterInvoke = IL.Create(OpCodes.Nop);
            // if (aroundInvoke != null ) {
            //      aroundInvoke.AfterInvoke(context, returnValue);
            instructions.Enqueue(IL.Create(OpCodes.Ldloc, _aroundInvoke));
            instructions.Enqueue(IL.Create(OpCodes.Ldloc, _context));
            instructions.Enqueue(IL.Create(OpCodes.Ldloc, _returnValue));
            instructions.Enqueue(IL.Create(OpCodes.Callvirt, _afterInvoke));
            // }

            if (returnType != _voidType)
            {
                instructions.Enqueue(IL.Create(OpCodes.Ldloc, _returnValue));
                IL.PackageReturnValue(module, instructions, returnType);
            }
            return instructions;
        }

    }
}
