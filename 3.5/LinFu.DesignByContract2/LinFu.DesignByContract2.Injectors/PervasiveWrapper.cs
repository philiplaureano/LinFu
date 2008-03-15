using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using LinFu.Common;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Injectors
{
    public sealed class PervasiveWrapper : IInvokeWrapper, ITypeWrapper 
    {
        private ProxyFactory _factory = new ProxyFactory();
        private IInvokeWrapper _wrapper;
        private ITypeWrapper _creator;
        public PervasiveWrapper(IInvokeWrapper wrapper) : this(wrapper, null)
        {
            _wrapper = wrapper;
            _creator = this;
        }

        public PervasiveWrapper(IInvokeWrapper wrapper, ITypeWrapper creator)
        {
            _wrapper = wrapper;
            _creator = creator;
        }

        #region IInvokeWrapper Members

        public void BeforeInvoke(InvocationInfo info)
        {
            _wrapper.BeforeInvoke(info);
        }

        public object DoInvoke(InvocationInfo info)
        {
            object returnValue = _wrapper.DoInvoke(info);
            
            if (_creator == null)
                return returnValue;
            
            if (info == null)
                return returnValue;
            
            Type returnType = info.TargetMethod.ReturnType;
            if (!_creator.CanWrap(returnType, returnValue))
                return returnValue;

            return _creator.Wrap(returnType, returnValue);
        }

        public void AfterInvoke(InvocationInfo info, object returnValue)
        {
            _wrapper.AfterInvoke(info, returnValue);
        }

        #endregion

        #region ITypeWrapper Members

        public bool CanWrap(Type targetType, object instance)
        {
            if (instance == null)
                return false;
            
            // Sealed types cannot be wrapped
            if (targetType.IsSealed)
                return false;
           
            // Only classes can be wrapped
            if (!targetType.IsClass)
                return false;
            
            // Interfaces can always be wrapped
            if (targetType.IsInterface)
                return true;

            // Search for any instance methods that are non-virtual
            Predicate<MethodInfo> criteria = null;
            criteria += delegate(MethodInfo method)
                            {
                                return !method.IsStatic && !method.IsVirtual;
                            };
          
            
            // Note: A type can be wrapped if and only if that type's methods
            // are all virtual
            FuzzyFinder<MethodInfo> finder = new FuzzyFinder<MethodInfo>();
            MethodInfo[] methods =
                targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            MethodInfo firstMatch = finder.Find(criteria, methods);
            
            if (firstMatch != null)
                return false;
            
            return true;
        }

        public object Wrap(Type type, object value)
        {
            return _factory.CreateProxy(type, new PervasiveWrapper(_wrapper, this));
        }

        #endregion
    }
}
