using System.Reflection;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    internal class InterceptNewCalls : InstructionSwapper, IMethodWeaver
    {
        private readonly INewObjectWeaver _emitter;
        private MethodReference _getCurrentMethod;

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

        protected override void Replace(Instruction currentInstruction, MethodDefinition method, ILProcessor IL)
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
            var declaringType = constructor.GetDeclaringType();
            return _emitter.ShouldIntercept(constructor, declaringType, hostMethod);
        }

        public bool ShouldWeave(MethodDefinition item)
        {
            return item.HasBody;
        }

        public void Weave(MethodDefinition item)
        {
            Rewrite(item, item.GetILGenerator(), item.Body.Instructions.ToArray());
        }
    }
}