using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection.Extensions
{
    public class CompositeMissingMethodCallback : BaseCompositeMethodMissingCallback
    {
        private List<IMethodMissingCallback> _callbacks = new List<IMethodMissingCallback>();

        public List<IMethodMissingCallback> Callbacks
        {
            get { return _callbacks; }
        }
        protected override IEnumerable<IMethodMissingCallback> GetCallbacks()
        {
            return _callbacks;
        }
    }
}
