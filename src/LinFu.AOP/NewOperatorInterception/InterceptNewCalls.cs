using System.Reflection;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    internal class InterceptNewCalls : InstructionSwapper
    {
        private MethodReference _getCurrentMethod;


        private readonly INewObjectWeaver _emitter;

        public InterceptNewCalls(INewObjectWeaver emitter)
        {
            _emitter = emitter;
        }

        public override void AddAdditionalMembers(TypeDefinition host)
        {
            _emitter.AddAdditionalMembers(host);
        }

        public override void ImportReferences(ModuleDefinition module)
        {
            _getCurrentMethod = module.ImportMethod<MethodBase>("GetCurrentMethod",
                BindingFlags.Public | BindingFlags.Static);
            _emitter.ImportReferences(module);
        }

        protected override void Replace(Instruction currentInstruction, MethodDefinition method, CilWorker IL)
        {
            var constructor = (MethodReference) currentInstruction.Operand;
            var concreteType = constructor.DeclaringType;
            var parameters = constructor.Parameters;

            if (!_emitter.ShouldIntercept(constructor, concreteType, method))
            {
                // Reuse the old instruction instead of emitting a new one
                IL.Append(currentInstruction);
                return;
            }

            _emitter.EmitNewObject(method, IL, constructor, concreteType);
        }

        public override void AddLocals(MethodDefinition hostMethod)
        {
            _emitter.AddLocals(hostMethod);
        }

        protected override bool ShouldReplace(Instruction oldInstruction, MethodDefinition hostMethod)
        {
            if (oldInstruction.OpCode != OpCodes.Newobj)
                return false;

            var constructor = (MethodReference) oldInstruction.Operand;
            TypeReference declaringType = constructor.GetDeclaringType();
            return _emitter.ShouldIntercept(constructor, declaringType, hostMethod);
        }
    }
}