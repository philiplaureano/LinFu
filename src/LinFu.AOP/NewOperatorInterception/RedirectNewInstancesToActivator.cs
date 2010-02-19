using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;
using LinFu.AOP.Interfaces;
using System.Reflection;

namespace LinFu.AOP.Cecil
{
    internal class RedirectNewInstancesToActivator : INewObjectWeaver
    {
        #region Private Fields
        private TypeReference _hostInterfaceType;
        private TypeReference _methodActivatorType;
        private TypeReference _objectType;
        private TypeReference _voidType;
        private TypeReference _objectListType;

        private MethodReference _getTypeFromHandle;
        private MethodReference _methodActivationContextCtor;
        private MethodReference _getActivator;
        private MethodReference _createInstance;
        private MethodReference _objectListCtor;
        private MethodReference _addMethod;
        private MethodReference _toArrayMethod;
        private MethodReference _reverseMethod;
        private MethodReference _getStaticActivator;
        private MethodReference _canActivate;
        private MethodReference _getItem;

        private VariableDefinition _constructorArguments;
        private VariableDefinition _currentArgument;
        private VariableDefinition _methodContext;
        private VariableDefinition _currentActivator;

        private Func<MethodReference, TypeReference, MethodReference, bool> _filter;

        #endregion


        public RedirectNewInstancesToActivator(Func<MethodReference, TypeReference, MethodReference, bool> filter)
        {
            _filter = filter;
        }

        public bool ShouldIntercept(MethodReference constructor, TypeReference concreteType, MethodReference hostMethod)
        {
            // Intercept all types by default
            if (_filter == null)
                return true;

            return _filter(constructor, concreteType, hostMethod);
        }

        public void AddAdditionalMembers(TypeDefinition host)
        {
            // Make sure the type implements IActivatorHost
            var interfaceWeaver = new ImplementActivatorHostWeaver();
            host.Accept(interfaceWeaver);
        }

        public void ImportReferences(ModuleDefinition module)
        {
            // Type imports
            _hostInterfaceType = module.ImportType<IActivatorHost>();
            _methodActivatorType = module.ImportType<ITypeActivator>();
            _objectType = module.ImportType<object>();
            _voidType = module.Import(typeof(void));
            _objectListType = module.ImportType<List<object>>();

            // Static method imports
            _getStaticActivator = module.ImportMethod("GetActivator", typeof(TypeActivatorRegistry), BindingFlags.Public | BindingFlags.Static);
            _getTypeFromHandle = module.ImportMethod<Type>("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static);

            // Constructor imports
            _methodActivationContextCtor = module.ImportConstructor<TypeActivationContext>(typeof(object), typeof(MethodBase), typeof(Type), typeof(object[]));

            // Instance method imports
            _getActivator = module.ImportMethod<IActivatorHost>("get_Activator");
            _objectListCtor = module.ImportConstructor<List<object>>(new Type[0]);
            _toArrayMethod = module.ImportMethod<List<object>>("ToArray", new Type[0]);
            _addMethod = module.ImportMethod<List<object>>("Add", new Type[] { typeof(object) });
            _reverseMethod = module.ImportMethod<List<object>>("Reverse", new Type[0]);
            _canActivate = module.ImportMethod<ITypeActivator>("CanActivate");
            _getItem = module.ImportMethod<List<object>>("get_Item", new Type[] { typeof(int) });

            var createInstanceMethod = typeof(IActivator<ITypeActivationContext>).GetMethod("CreateInstance");

            _createInstance = module.Import(createInstanceMethod);
        }

        public void EmitNewObject(MethodDefinition hostMethod, CilWorker IL, MethodReference targetConstructor, TypeReference concreteType)
        {
            var parameters = targetConstructor.Parameters;
            Instruction skipInterception = IL.Create(OpCodes.Nop);

            SaveConstructorArguments(IL, parameters);
            EmitCreateMethodActivationContext(hostMethod, IL, concreteType);

            // Skip the interception if an activator cannot be found            
            EmitGetActivator(hostMethod, IL, skipInterception);

            IL.Emit(OpCodes.Stloc, _currentActivator);            
            
            IL.Emit(OpCodes.Ldloc, _currentActivator);
            IL.Emit(OpCodes.Brfalse, skipInterception);

            // Determine if the activator can instantiate the method from the
            // current context
            IL.Emit(OpCodes.Ldloc, _currentActivator);
            IL.Emit(OpCodes.Ldloc, _methodContext);
            IL.Emit(OpCodes.Callvirt, _canActivate);
            IL.Emit(OpCodes.Brfalse, skipInterception);

            // Use the activator to create the object instance
            EmitCreateInstance(IL);

            var skipWriteLine = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Dup);
            IL.Emit(OpCodes.Brtrue, skipWriteLine);
            IL.EmitWriteLine("DEBUG: The instance is null");
            IL.Append(skipWriteLine);

            // }
            Instruction endCreate = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Br, endCreate);
            // else {
            IL.Append(skipInterception);

            // Restore the arguments that were popped off the stack
            // by the list of constructor arguments
            var parameterCount = parameters.Count;
            for (var index = 0; index < parameterCount; index++)
            {
                var currentParameter = parameters[index];

                IL.Emit(OpCodes.Ldloc, _constructorArguments);
                IL.Emit(OpCodes.Ldc_I4, index);
                IL.Emit(OpCodes.Callvirt, _getItem);
                IL.Emit(OpCodes.Unbox_Any, currentParameter.ParameterType);
            }

            IL.Emit(OpCodes.Newobj, targetConstructor);
            // }

            IL.Append(endCreate);
        }

        private void EmitCreateInstance(CilWorker IL)
        {
            // T instance = this.Activator.CreateInstance(context);
            IL.Emit(OpCodes.Ldloc, _currentActivator);
            IL.Emit(OpCodes.Ldloc, _methodContext);
            IL.Emit(OpCodes.Callvirt, _createInstance);
        }

        private void EmitCreateMethodActivationContext(MethodDefinition method, CilWorker IL, TypeReference concreteType)
        {
            // TODO: Add static method support
            var pushThis = method.IsStatic ? IL.Create(OpCodes.Ldnull) : IL.Create(OpCodes.Ldarg_0);

            // Push the 'this' pointer onto the stack
            IL.Append(pushThis);

            var module = method.DeclaringType.Module;

            // Push the current method onto the stack
            IL.PushMethod(method, module);

            // Push the concrete type onto the stack
            IL.Emit(OpCodes.Ldtoken, concreteType);
            IL.Emit(OpCodes.Call, _getTypeFromHandle);

            IL.Emit(OpCodes.Ldloc, _constructorArguments);
            IL.Emit(OpCodes.Callvirt, _toArrayMethod);
            IL.Emit(OpCodes.Newobj, _methodActivationContextCtor);

            // var context = new MethodActivationContext(this, currentMethod, concreteType, args);
            IL.Emit(OpCodes.Stloc, _methodContext);
        }

        private void SaveConstructorArguments(CilWorker IL, ParameterDefinitionCollection parameters)
        {
            var parameterCount = parameters.Count;

            IL.Emit(OpCodes.Newobj, _objectListCtor);
            IL.Emit(OpCodes.Stloc, _constructorArguments);

            var index = parameterCount - 1;
            while (index >= 0)
            {
                var param = parameters[index];

                SaveConstructorArgument(IL, param);

                index--;
            }

            // Reverse the constructor arguments so that they appear in the correct order
            IL.Emit(OpCodes.Ldloc, _constructorArguments);
            IL.Emit(OpCodes.Callvirt, _reverseMethod);
        }

        private void SaveConstructorArgument(CilWorker IL, ParameterDefinition param)
        {
            // Box the type if necessary
            var parameterType = param.ParameterType;
            if (parameterType.IsValueType || parameterType is GenericParameter)
                IL.Emit(OpCodes.Box, parameterType);

            // Save the current argument
            IL.Emit(OpCodes.Stloc, _currentArgument);

            // Add the item to the item to the collection
            IL.Emit(OpCodes.Ldloc, _constructorArguments);
            IL.Emit(OpCodes.Ldloc, _currentArgument);
            IL.Emit(OpCodes.Callvirt, _addMethod);
        }

        private void EmitGetActivator(MethodDefinition method, CilWorker IL, Instruction skipInterception)
        {            
            IL.Emit(OpCodes.Ldloc, _methodContext);
            IL.Emit(OpCodes.Call, _getStaticActivator);
        }

        public void AddLocals(MethodDefinition hostMethod)
        {
            _constructorArguments = hostMethod.AddLocal<List<object>>();
            _currentArgument = hostMethod.AddLocal<object>();
            _methodContext = hostMethod.AddLocal<ITypeActivationContext>();
            _currentActivator = hostMethod.AddLocal<ITypeActivator>();
        }
    }
}
