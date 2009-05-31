using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;

namespace SampleLibrary.AOP
{
    public class SampleTypeActivator : ITypeActivator
    {
        private Func<ITypeActivationContext, object> _createInstance;
        public SampleTypeActivator(Func<ITypeActivationContext, object> createInstance)
        {
            _createInstance = createInstance;
        }

        public bool CanActivate(ITypeActivationContext context)
        {
            return true;
        }

        public object CreateInstance(ITypeActivationContext context)
        {
            return _createInstance(context);
        }
    }
}
