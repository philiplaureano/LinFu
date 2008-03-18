using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LinFu.Delegates
{
    public class Closure : IDeferredArgument 
    {
        private MulticastDelegate _target;
        private IInvoker _invoker = new DefaultInvoker();
        private readonly List<object> _suppliedArguments = new List<object>();
        public Closure()
        {

        }
        public Closure(CustomDelegate body, Type returnType, 
            Type[] parameterTypes, params object[] suppliedArguments)
        {
            MulticastDelegate targetDelegate = DelegateFactory.DefineDelegate(body, returnType, parameterTypes);
            _target = targetDelegate;

            if (suppliedArguments == null || suppliedArguments.Length == 0)
                return;

            _suppliedArguments.AddRange(suppliedArguments);
        }
        public Closure(CustomDelegate target, params object[] suppliedArguments)
            : this((MulticastDelegate)target, suppliedArguments)
        {
            CustomDelegate customDelegate = (CustomDelegate)target;
            _invoker = new CustomDelegateInvoker(customDelegate, suppliedArguments);
        }
        public Closure(MethodInfo staticMethod, 
            params object[] suppliedArguments) 
        {
            if (!staticMethod.IsStatic)
                throw new ArgumentException("The target method must be static and it cannot be an instance method",
                                            "staticMethod");

            MulticastDelegate target = DelegateFactory.DefineDelegate(null, staticMethod);
            _target = target;

            if (suppliedArguments == null || suppliedArguments.Length == 0)
                return;

            _suppliedArguments.AddRange(suppliedArguments);
        }

        public Closure(object instance, MethodInfo targetMethod, 
            params object[] suppliedArguments)
        {
            MulticastDelegate target = DelegateFactory.DefineDelegate(instance, targetMethod);
            _target = target;

            if (suppliedArguments == null || suppliedArguments.Length == 0)
                return;

            _suppliedArguments.AddRange(suppliedArguments);
        }
        public Closure(MulticastDelegate target)
        {
            _target = target;
        }       
        public Closure(MulticastDelegate target, params object[] suppliedArguments)
        {
            _target = target;

            if (suppliedArguments == null || suppliedArguments.Length == 0)
                return;


            _suppliedArguments.AddRange(suppliedArguments);
        }
        public List<object> Arguments
        {
            get { return _suppliedArguments;  }   
        }
        public MulticastDelegate Target
        {
           get
           {
               return _target;
           }
           set
           {
               _target = value;
           }
        }

        public IInvoker Invoker
        {
            get { return _invoker; }
            set { _invoker = value; }
        }

        public object Invoke(params object[] args)
        {
            if (_target == null)
                throw new NotImplementedException();

            if (_invoker == null)
                throw new NotImplementedException();
           
            return _invoker.Invoke(_target.Target, _target.Method, _suppliedArguments, args);
        }
        
        #region IDeferredArgument Members

        public object Evaluate()
        {
            return Invoke();
        }

        #endregion

        public TDelegate AdaptTo<TDelegate>()
            where TDelegate : class
        {
            return AdaptTo(typeof (TDelegate)) as TDelegate;
        }
        public MulticastDelegate AdaptTo(Type delegateType)
        {
            if (!typeof(MulticastDelegate).IsAssignableFrom(delegateType))
                throw new ArgumentException("Generic parameter 'TDelegate' must be derived from MulticastDelegate");

            // Create a 'fake' delegate that redirects its
            // calls back to this closure
            CustomDelegate body = delegate(object[] args)
                                      {
                                          return Invoke(args);
                                      };

            
            MulticastDelegate result = DelegateFactory.DefineDelegate(delegateType, body);
            return result;
        }
    }
}
