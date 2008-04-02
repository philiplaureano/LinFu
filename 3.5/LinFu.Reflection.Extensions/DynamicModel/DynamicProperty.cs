using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection.Extensions
{
    public class DynamicProperty : BaseCompositeMethodMissingCallback
    {
        public DynamicProperty()
        {
        }

        public PropertySpec PropertySpec
        {
            get;
            set;
        }

        protected override IEnumerable<IMethodMissingCallback> GetCallbacks()
        {
            var setCallback = new PropertySetterCallback(PropertySpec);
            var getCallback = new PropertyGetterCallback(PropertySpec);

            return new IMethodMissingCallback[] { setCallback, getCallback };
        }
    }
}
