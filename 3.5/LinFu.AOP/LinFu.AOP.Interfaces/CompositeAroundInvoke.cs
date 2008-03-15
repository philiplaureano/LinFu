using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    public class CompositeAroundInvoke : IAroundInvoke
    {
        private IList<IAroundInvoke> _aroundInvokeList = new List<IAroundInvoke>();
        public CompositeAroundInvoke(IEnumerable<IAroundInvoke> aroundInvokeList)
        {
            if (aroundInvokeList == null)
                throw new ArgumentNullException("aroundInvokeList");

            // Filter out the null values
            foreach (var current in aroundInvokeList)
            {
                if (current == null)
                    continue;

                _aroundInvokeList.Add(current);
            }
        }
        #region IAroundInvoke Members

        public void AfterInvoke(IInvocationContext context, object returnValue)
        {

            foreach (var invoke in _aroundInvokeList)
            {
                invoke.AfterInvoke(context, returnValue);
            }            
        }

        public void BeforeInvoke(IInvocationContext context)
        {
            foreach (var invoke in _aroundInvokeList)
            {
                invoke.BeforeInvoke(context);
            }            
        }

        #endregion
        public IList<IAroundInvoke> AroundInvokeList
        {
            get { return _aroundInvokeList; }
        }
    }
}
