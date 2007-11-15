using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace LinFu.DynamicProxy
{
    internal class DefaultyProxyMethodBuilder : IProxyMethodBuilder
    {
        private IMethodBodyEmitter _emitter;

        public DefaultyProxyMethodBuilder() : this(new DefaultMethodEmitter())
        {
        }

        public DefaultyProxyMethodBuilder(IMethodBodyEmitter emitter)
        {
            _emitter = emitter;
        }

        public IMethodBodyEmitter MethodBodyEmitter
        {
            get { return _emitter; }
            set { _emitter = value; }
        }

        #region IProxyMethodBuilder Members

        public void CreateProxiedMethod(FieldInfo field, MethodInfo method, TypeBuilder typeBuilder)
        {
            ParameterInfo[] parameters = method.GetParameters();
            List<Type> parameterTypes = new List<Type>();
            foreach (ParameterInfo param in parameters)
            {
                parameterTypes.Add(param.ParameterType);
            }

            MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig |
                                                MethodAttributes.Virtual;
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(method.Name, methodAttributes,
                                                                   CallingConventions.HasThis, method.ReturnType,
                                                                   parameterTypes.ToArray());

            Type[] typeArgs = method.GetGenericArguments();

            if (typeArgs != null && typeArgs.Length > 0)
            {
                List<string> typeNames = new List<string>();

                for (int index = 0; index < typeArgs.Length; index++)
                {
                    typeNames.Add(string.Format("T{0}", index));
                }

                methodBuilder.DefineGenericParameters(typeNames.ToArray());
            }

            ILGenerator IL = methodBuilder.GetILGenerator();

            Debug.Assert(_emitter != null);
            _emitter.EmitMethodBody(IL, method, field);
        }

        #endregion
    }
}