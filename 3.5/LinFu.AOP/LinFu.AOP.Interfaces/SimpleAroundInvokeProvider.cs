using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    public class SimpleAroundInvokeProvider : IAroundInvokeProvider 
    {
        private IAroundInvoke _around;
        private Predicate<IInvocationContext> _predicate;
        public SimpleAroundInvokeProvider(IAroundInvoke around)
        {
            _around = around;
        }
        public SimpleAroundInvokeProvider(IAroundInvoke around, Predicate<IInvocationContext> predicate)
        {
            _around = around;
            _predicate = predicate;
        }
        public Predicate<IInvocationContext> Predicate
        {
            get { return _predicate;  }
            set { _predicate = value; }
        }
        #region IAroundInvokeProvider Members

        public IAroundInvoke GetSurroundingImplementation(IInvocationContext context)
        {
            if (_predicate == null)
                return _around;

            // Apply the predicate, if possible
            if (_predicate(context))
                return _around;

            return null;
        }

        #endregion
    }
}
