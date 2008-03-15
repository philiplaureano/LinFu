using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DesignByContract2.Core;
using Simple.IoC;
using Simple.IoC.Loaders;

namespace LinFu.DesignByContract2.Injectors
{
    [Implements(typeof(IContractChecker), LifecycleType.OncePerRequest)]
    public class AutoContractChecker : ContractChecker, IInitialize 
    {
        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            ContractProvider = container.GetService<IContractProvider>();
        }

        #endregion
    }
}
