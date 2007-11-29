using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;
using Simple.IoC;
using Simple.IoC.Loaders;

namespace LinFu.DesignByContract2.Injectors
{
    [Implements(typeof(ITypeInjector), LifecycleType.Singleton)]
    public class ContractInjector : ITypeInjector, IInitialize
    {
        private IContainer _container;
        private IContractStorage _storage;
        private ProxyFactory factory = new ProxyFactory();
        #region IInjector Members
        public ContractInjector()
        {
            
        }
        public ContractInjector(IContainer container)
        {
            Initialize(container);
        }
        public bool CanInject(Type serviceType, object instance)
        {                                    
            Debug.Assert(_storage != null);
            
            // None of the types in this assembly should ever be injected
            if (serviceType.Assembly == typeof(ContractInjector).Assembly)
                return false;
            
            if (_storage == null || !_storage.HasContractFor(serviceType))
                return false;
            
            return true;
        }

        public object Inject(Type serviceType, object instance)
        {
            IContractChecker checker = _container.GetService<IContractChecker>();

            Debug.Assert(checker != null);
            checker.Target = instance;

#if !DISABLE_PERVASIVE_WRAPPING
            PervasiveWrapper wrapper = new PervasiveWrapper(checker);
            object result = factory.CreateProxy(serviceType, wrapper, new Type[0]);
#else            
            object result = factory.CreateProxy(serviceType, (IInvokeWrapper)checker, new Type[0]);
#endif
            return result;
        }

        #endregion

        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            if (_storage == null)
                _storage = container.GetService<IContractStorage>();
            
            if (_container == null)
                _container = container;
            
            Attach(container);
        }

        #endregion
        
        public void Attach(IContainer container)
        {
            if (container.TypeInjectors.Contains(this))
                return;

            container.TypeInjectors.Add(this);
        }
    }
}
