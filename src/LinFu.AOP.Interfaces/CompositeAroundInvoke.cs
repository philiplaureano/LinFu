using System;
using System.Collections.Generic;

namespace LinFu.AOP.Interfaces
{
    internal class CompositeAroundInvoke : IAroundInvoke
    {
        private readonly IList<IAroundInvoke> _aroundInvokeList = new List<IAroundInvoke>();

        public CompositeAroundInvoke(IEnumerable<IAroundInvoke> aroundInvokeList)
        {
            if (aroundInvokeList == null)
                throw new ArgumentNullException("aroundInvokeList");

            // Filter out the null values
            foreach (IAroundInvoke current in aroundInvokeList)
            {
                if (current == null)
                    continue;

                _aroundInvokeList.Add(current);
            }
        }

        #region IAroundInvoke Members

        public void AfterInvoke(IInvocationInfo context, object returnValue)
        {
            foreach (IAroundInvoke invoke in _aroundInvokeList)
            {
                invoke.AfterInvoke(context, returnValue);
            }
        }

        public void BeforeInvoke(IInvocationInfo context)
        {
            foreach (IAroundInvoke invoke in _aroundInvokeList)
            {
                invoke.BeforeInvoke(context);
            }
        }

        #endregion
    }
}