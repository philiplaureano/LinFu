using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DesignByContract2.Attributes;
using LinFu.DesignByContract2.Core;
using Simple.IoC;
using Simple.IoC.Loaders;

namespace LinFu.DesignByContract2.Injectors
{
    [Implements(typeof(IContractProvider), LifecycleType.OncePerRequest)]
    public class ExternalContractProvider : AttributeContractProvider, IInitialize
    {

        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            ContractTypeProvider = container.GetService<IContractTypeProvider>();
        }

        #endregion
    }
}
