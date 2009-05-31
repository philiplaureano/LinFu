using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil.Cil;
using Mono.Cecil;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using System.Reflection;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents a <see cref="MethodRewriter"/> that intercepts calls to field getters and setters and redirects those calls to
    /// a <see cref="IFieldInterceptor"/> instance.
    /// </summary>
    internal class InterceptFieldAccess : MethodRewriter
    {
        private static readonly HashSet<OpCode> _fieldInstructions = new HashSet<OpCode>();
        private TypeReference _fieldContextType;
        private TypeReference _fieldInterceptionHostType;

        private MethodReference _fieldContextCtor;
        private MethodReference _getFieldFromHandle;
        private MethodReference _getType;
        private MethodReference _getInterceptor;
        private MethodReference _getInstanceInterceptor;
        private MethodReference _canIntercept;
        private MethodReference _getValue;
        private MethodReference _setValue;

        private VariableDefinition _fieldContext;
        private VariableDefinition _fieldInterceptor;
        private VariableDefinition _currentArgument;

        private Func<FieldReference, bool> _filter;


        static InterceptFieldAccess()
        {
            _fieldInstructions.Add(OpCodes.Ldsfld);
            _fieldInstructions.Add(OpCodes.Ldfld);
            _fieldInstructions.Add(OpCodes.Stsfld);
            _fieldInstructions.Add(OpCodes.Stfld);
        }

        /// <summary>
        /// Initializes a new instance of the InterceptFieldAccess class.
        /// </summary>
        public InterceptFieldAccess()
        {
            // Weave all fields by default
            _filter = field => true;
        }

        /// <summary>
        /// Initializes a new instance of the InterceptFieldAccess class.
        /// </summary>
        /// <param name="filter">The filter that determines which fields should be intercepted.</param>
        public InterceptFieldAccess(Func<FieldReference, bool> filter)
        {
            _filter = filter;
        }

        /// <summary>
        /// Adds locals to the target method.
        /// </summary>
        /// <param name="hostMethod">The method to be modified</param>
        public override void AddLocals(MethodDefinition hostMethod)
        {
            _fieldContext = hostMethod.AddLocal<IFieldInterceptionContext>("__<>FieldInterceptionContext<>__");
            _fieldInterceptor = hostMethod.AddLocal<IFieldInterceptor>("__<>FieldInterceptor<>__");
            _currentArgument = hostMethod.AddLocal<object>("__<>CurrentArgument<>__");
        }

        /// <summary>
        /// Adds references to the target module.
        /// </summary>
        /// <param name="module">The module that will be modified.</param>
        public override void ImportReferences(ModuleDefinition module)
        {
            var parameterTypes = new Type[] { typeof(object), typeof(MethodBase), typeof(FieldInfo), typeof(Type) };
            _fieldContextType = module.ImportType<IFieldInterceptionContext>();
            _fieldInterceptionHostType = module.ImportType<IFieldInterceptionHost>();

            _fieldContextCtor = module.ImportConstructor<FieldInterceptionContext>(parameterTypes);
            _getFieldFromHandle = module.ImportMethod<FieldInfo>("GetFieldFromHandle", new Type[] { typeof(RuntimeFieldHandle), typeof(RuntimeTypeHandle) });
            _getType = module.ImportMethod<object>("GetType");
            _getInterceptor = module.ImportMethod<FieldInterceptorRegistry>("GetInterceptor");
            _getInstanceInterceptor = module.ImportMethod<IFieldInterceptionHost>("get_FieldInterceptor");
            _canIntercept = module.ImportMethod<IFieldInterceptor>("CanIntercept");

            _getValue = module.ImportMethod<IFieldInterceptor>("GetValue");
            _setValue = module.ImportMethod<IFieldInterceptor>("SetValue");
        }

        /// <summary>
        /// Determines whether or not the method rewriter should replace the <paramref name="oldInstruction"/>.
        /// </summary>
        /// <remarks>The <see cref="InterceptFieldAccess"/> class only modifies instructions that get or set the value of static and instance fields.</remarks>
        /// <param name="oldInstruction">The instruction that is currently being evaluated.</param>
        /// <param name="hostMethod">The method that hosts the current instruction.</param>
        /// <returns><c>true</c> if the method should be replaced; otherwise, it should return <c>false</c>.</returns>
        protected override bool ShouldReplace(Instruction oldInstruction, MethodDefinition hostMethod)
        {
            if (!_fieldInstructions.Contains(oldInstruction.OpCode))
                return false;

            // Match the field filter
            var targetField = (FieldReference)oldInstruction.Operand;
            return _filter(targetField);
        }

        /// <summary>
        /// Replaces the <paramref name="oldInstruction"/> with a set of <paramref name="newInstructions"/>.
        /// </summary>
        /// <param name="oldInstruction">The instruction currently being evaluated.</param>
        /// <param name="hostMethod">The method that contains the target instruction.</param>
        /// <param name="IL">The CilWorker that will be used to emit the method body instructions.</param>
        protected override void Replace(Instruction oldInstruction, MethodDefinition hostMethod, CilWorker IL)
        {
            var targetField = (FieldReference)oldInstruction.Operand;
            var fieldType = targetField.FieldType;
            var isSetter = oldInstruction.OpCode == OpCodes.Stsfld || oldInstruction.OpCode == OpCodes.Stfld;

            if (isSetter)
            {
                hostMethod.Body.InitLocals = true;
                // Save the setter argument and box it if necessary
                if (fieldType.IsValueType || fieldType is GenericParameter)
                    IL.Emit(OpCodes.Box, fieldType);

                IL.Emit(OpCodes.Stloc, _currentArgument);
            }

            // There's no need to push the current object instance
            // since the this pointer is pushed prior to the field call
            if (hostMethod.IsStatic)
                IL.Emit((OpCodes.Ldnull));

            // Push the current method
            var module = hostMethod.DeclaringType.Module;

            // Push the current method onto the stack
            IL.PushMethod(hostMethod, module);

            // Push the current field onto the stack            
            IL.PushField(targetField, module);

            // Push the host type onto the stack
            IL.PushType(hostMethod.DeclaringType, module);

            // Create the IFieldInterceptionContext instance
            IL.Emit(OpCodes.Newobj, _fieldContextCtor);
            IL.Emit(OpCodes.Stloc, _fieldContext);

            var skipInterception = IL.Create(OpCodes.Nop);
            // Obtain an interceptor instance
            if (hostMethod.IsStatic)
            {
                IL.Emit(OpCodes.Ldloc, _fieldContext);
                IL.Emit(OpCodes.Call, _getInterceptor);
            }
            else
            {
                IL.Emit(OpCodes.Ldarg_0);
                IL.Emit(OpCodes.Isinst, _fieldInterceptionHostType);
                IL.Emit(OpCodes.Brfalse, skipInterception);

                IL.Emit(OpCodes.Ldarg_0);
                IL.Emit(OpCodes.Isinst, _fieldInterceptionHostType);
                IL.Emit(OpCodes.Callvirt, _getInstanceInterceptor);
            }

            // The field interceptor cannot be null
            IL.Emit(OpCodes.Stloc, _fieldInterceptor);
            IL.Emit(OpCodes.Ldloc, _fieldInterceptor);
            IL.Emit(OpCodes.Brfalse, skipInterception);

            // if (FieldInterceptor.CanIntercept(context) {
            IL.Emit(OpCodes.Ldloc, _fieldInterceptor);
            IL.Emit(OpCodes.Ldloc, _fieldContext);
            IL.Emit(OpCodes.Callvirt, _canIntercept);
            IL.Emit(OpCodes.Brfalse, skipInterception);

            var isGetter = oldInstruction.OpCode == OpCodes.Ldsfld || oldInstruction.OpCode == OpCodes.Ldfld;

            var endLabel = IL.Create(OpCodes.Nop);

            //Call the interceptor instead of the getter or setter
            if (isGetter)
            {
                IL.Emit(OpCodes.Ldloc, _fieldInterceptor);
                IL.Emit(OpCodes.Ldloc, _fieldContext);
                IL.Emit(OpCodes.Callvirt, _getValue);
                IL.Emit(OpCodes.Unbox_Any, fieldType);
            }

            if (isSetter)
            {
                // Push the 'this' pointer for instance field setters
                if (!hostMethod.IsStatic)
                    IL.Emit(OpCodes.Ldarg_0);

                IL.Emit(OpCodes.Ldloc, _fieldInterceptor);
                IL.Emit(OpCodes.Ldloc, _fieldContext);
                IL.Emit(OpCodes.Ldloc, _currentArgument);

                // Unbox the setter value
                IL.Emit(OpCodes.Unbox_Any, fieldType);

                IL.Emit(OpCodes.Callvirt, _setValue);

                // Set the actual field value
                IL.Emit(OpCodes.Unbox_Any, fieldType);
                IL.Emit(oldInstruction.OpCode, targetField);
            }

            IL.Emit(OpCodes.Br, endLabel);

            // }
            IL.Append(skipInterception);

            // else {

            // Load the original field
            if (!hostMethod.IsStatic)
                IL.Emit(OpCodes.Ldarg_0);


            if (isSetter)
            {
                IL.Emit(OpCodes.Ldloc, _currentArgument);

                // Unbox the setter value
                IL.Emit(OpCodes.Unbox_Any, fieldType);
            }

            IL.Emit(oldInstruction.OpCode, targetField);

            // }

            IL.Append(endLabel);
        }
    }
}
