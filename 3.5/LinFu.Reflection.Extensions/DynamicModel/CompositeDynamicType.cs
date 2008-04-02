using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection.Extensions
{
    internal class CompositeDynamicType : DynamicType
    {
        private readonly IEnumerable<DynamicType> _types;
        public CompositeDynamicType(IEnumerable<DynamicType> types)
        {
            _types = types;
        }
        protected override IEnumerable<IMethodMissingCallback> GetCallbacks()
        {
            return _types.Cast<IMethodMissingCallback>();
        }
    }
}
