using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Simple.IoC.Loaders;
using Simple.IoC;

namespace LinFu.DesignByContract2.Injectors
{
    [Implements(typeof(IContractLoader), LifecycleType.Singleton)]    
    public class ContractLoader : BaseLoader, IContractLoader, IInitialize
    {   
        public override void LoadDirectory(string directory, string filespec)
        {
            Debug.Assert(Container != null);
            LoadStrategy = new LoadContractStrategy();
            base.LoadDirectory(directory, filespec);
        }
        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            Container = container;
        }

        #endregion
    }
}
