using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.CecilExtensions
{
    public abstract class MethodPrologEpilogWeaver : IMethodWeaver
    {
        #region IWeaver<MethodDefinition> Members

        public abstract bool ShouldWeave(MethodDefinition item);
        public virtual void ImportReferences(ModuleDefinition module) { }
        public virtual void Weave(MethodDefinition item)
        {
            if (item == null)
                return;

            Mono.Cecil.Cil.MethodBody methodBody = item.Body;
            if (methodBody == null)
                return;

            methodBody.InitLocals = true;
            AddLocals(item);

            List<Instruction> originalInstructions = new List<Instruction>();
            foreach (Instruction current in methodBody.Instructions)
            {
                if (current.OpCode == OpCodes.Ret)
                    continue;

                originalInstructions.Add(current);
            }

            methodBody.Instructions.Clear();
            IEnumerable<Instruction> prolog = CreateProlog(item, originalInstructions);
            IEnumerable<Instruction> epilog = CreateEpilog(item, originalInstructions);

            CilWorker IL = methodBody.CilWorker;
            IL.AppendInstructions(prolog);
            IL.AppendInstructions(originalInstructions);
            IL.AppendInstructions(epilog);
            IL.Emit(OpCodes.Ret);
        }

        #endregion

        public virtual IEnumerable<Instruction> CreateProlog(MethodDefinition methodDef, IEnumerable<Instruction> originalInstructions)
        {
            return new Instruction[0];
        }
        public virtual IEnumerable<Instruction> CreateEpilog(MethodDefinition methodDef, IEnumerable<Instruction> originalInstructions)
        {
            return new Instruction[0];
        }
        public virtual void AddLocals(MethodDefinition item)
        {

        }

        #region IMethodWeaver Members

        public virtual void AddAdditionalMembers(TypeDefinition typeDef)
        {

        }

        #endregion
    }
}
