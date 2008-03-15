using System;
using System.Reflection;
using System.Reflection.Emit;

namespace LinFu.DynamicProxy
{
    internal class ProxyImplementor
    {
        private FieldBuilder field;

        public FieldBuilder InterceptorField
        {
            get { return field; }
        }

        public void ImplementProxy(TypeBuilder typeBuilder)
        {
            // Implement the IProxy interface
            typeBuilder.AddInterfaceImplementation(typeof (IProxy));

            field = typeBuilder.DefineField("__interceptor", typeof (IInterceptor),
                                            FieldAttributes.Private);

            // Implement the getter
            MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig |
                                          MethodAttributes.SpecialName | MethodAttributes.NewSlot |
                                          MethodAttributes.Virtual;

            // Implement the getter
            MethodBuilder getterMethod = typeBuilder.DefineMethod("get_Interceptor", attributes,
                                                                  CallingConventions.HasThis, typeof (IInterceptor),
                                                                  new Type[0]);
            getterMethod.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.IL);

            ILGenerator IL = getterMethod.GetILGenerator();

            // This is equivalent to:
            // get { return __interceptor;
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Ldfld, field);
            IL.Emit(OpCodes.Ret);

            // Implement the setter
            MethodBuilder setterMethod = typeBuilder.DefineMethod("set_Interceptor", attributes,
                                                                  CallingConventions.HasThis, typeof (void),
                                                                  new Type[] {typeof (IInterceptor)});

            setterMethod.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.IL);
            IL = setterMethod.GetILGenerator();
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Ldarg_1);
            IL.Emit(OpCodes.Stfld, field);
            IL.Emit(OpCodes.Ret);

            MethodInfo originalSetter = typeof (IProxy).GetMethod("set_Interceptor");
            MethodInfo originalGetter = typeof (IProxy).GetMethod("get_Interceptor");

            typeBuilder.DefineMethodOverride(setterMethod, originalSetter);
            typeBuilder.DefineMethodOverride(getterMethod, originalGetter);
        }
    }
}