using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            foreach (var current in aroundInvokeList)
            {
                if (current == null)
                    continue;

                _aroundInvokeList.Add(current);
            }
        }

        public void AfterInvoke(IInvocationInfo context, object returnValue)
        {

            foreach (var invoke in _aroundInvokeList)
            {
                invoke.AfterInvoke(context, returnValue);
            }
        }

        public void BeforeInvoke(IInvocationInfo context)
        {
            foreach (var invoke in _aroundInvokeList)
            {
                invoke.BeforeInvoke(context);
            }
        }     
    }
}
