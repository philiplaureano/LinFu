using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    public class SimpleMethodReplacementProvider : BaseMethodReplacementProvider
    {
        public SimpleMethodReplacementProvider(IMethodReplacement replacement)
        {
            MethodReplacement = replacement;
        }
        public Predicate<IInvocationContext> MethodReplacementPredicate
        {
            get;
            set;
        }
        public IMethodReplacement MethodReplacement
        {
            get;
            set;
        }
        protected override bool ShouldReplace(IInvocationContext context)
        {
            if (MethodReplacementPredicate == null)
                return true;

            return MethodReplacementPredicate(context);
        }
        protected override IMethodReplacement GetReplacement(IInvocationContext context)
        {
            return MethodReplacement;
        }
    }
}
