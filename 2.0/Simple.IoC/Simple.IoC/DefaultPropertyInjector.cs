using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.IoC
{
    public class DefaultPropertyInjector : IPropertyInjector 
    {
        #region IPropertyInjector Members

        public bool CanInject(object instance, IContainer sourceContainer)
        {
            return instance is IInitialize;
        }

        public void InjectProperties(object instance, IContainer sourceContainer)
        {
            // Initialize the instance, if necessary
            if (!(instance is IInitialize))
                return;

            IInitialize init = instance as IInitialize;
            init.Initialize(sourceContainer);
        }
        #endregion
    }
}
