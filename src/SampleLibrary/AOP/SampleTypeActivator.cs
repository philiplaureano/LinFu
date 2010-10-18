using System;
using LinFu.AOP.Interfaces;

namespace SampleLibrary.AOP
{
    public class SampleTypeActivator : ITypeActivator
    {
        private readonly Func<ITypeActivationContext, object> _createInstance;

        public SampleTypeActivator(Func<ITypeActivationContext, object> createInstance)
        {
            _createInstance = createInstance;
        }

        #region ITypeActivator Members

        public bool CanActivate(ITypeActivationContext context)
        {
            return true;
        }

        public object CreateInstance(ITypeActivationContext context)
        {
            return _createInstance(context);
        }

        #endregion
    }
}