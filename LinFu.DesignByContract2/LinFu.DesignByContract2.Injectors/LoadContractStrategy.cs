using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using LinFu.DesignByContract2.Attributes;
using Simple.IoC;
using Simple.IoC.Loaders.Interfaces;

namespace LinFu.DesignByContract2.Injectors
{
    internal class LoadContractStrategy : ILoadStrategy
    {
        #region ILoadStrategy Members

        public void ProcessLoadedTypes(IContainer hostContainer, List<Type> loadedTypes)
        {
            IContractStorage storage = hostContainer.GetService<IContractStorage>();
            Debug.Assert(storage != null);
            if (storage == null)
                return;
            
            foreach(Type currentType in loadedTypes)
            {
                object[] attributes = currentType.GetCustomAttributes(typeof (ContractForAttribute), false);
                if (attributes == null || attributes.Length == 0)
                    continue;
                
                foreach(ContractForAttribute attribute in attributes)
                {
                    storage.AddContractType(attribute.TargetType, new TypeContractSource(currentType));
                }                
            }
            
            // Attach the contract injector to the container
            ContractInjector injector = new ContractInjector(hostContainer);
            injector.Attach(hostContainer);
        }

        #endregion
    }
}
